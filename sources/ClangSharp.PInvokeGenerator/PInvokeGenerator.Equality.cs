// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.CSharp;

namespace ClangSharp;

public partial class PInvokeGenerator
{
    private enum EqualityFieldKind
    {
        Value,
        Pointer,
        Record,
    }

    // Collects the instance fields that a field-wise equality implementation would compare, or returns
    // null when the record is not a safe candidate for `generate-equality-methods`. Element-wise equality
    // is only ever opt-in because it is not universally valid (padding, exported comparison functions),
    // so anything that cannot be compared field-for-field here is left untouched rather than guessed at.
    private List<(string Name, EqualityFieldKind Kind)>? GetEqualityFields(RecordDecl recordDecl, CXXRecordDecl? cxxRecordDecl)
    {
        if (!_config.GenerateEqualityMethods)
        {
            return null;
        }

        var fields = new List<(string Name, EqualityFieldKind Kind)>();

        // Eligibility is transitive: the emitted Equals(TSelf) calls each nested struct's own Equals, so
        // if any record anywhere in the layout can't be compared field-for-field the whole thing falls
        // back to reflection-based ValueType.Equals (padding-sensitive and slow), which is exactly what
        // this feature exists to avoid. Reject the outer type unless the full transitive layout is safe.
        return IsRecordEqualityEligible(recordDecl, cxxRecordDecl, fields, []) && (fields.Count != 0) ? fields : null;
    }

    private bool IsRecordEqualityEligible(RecordDecl recordDecl, CXXRecordDecl? cxxRecordDecl, List<(string Name, EqualityFieldKind Kind)>? fields, HashSet<string> visited)
    {
        // Unions have overlapping storage so there is no meaningful field-wise comparison.
        if (recordDecl.IsUnion)
        {
            return false;
        }

        // Vtbl/COM types compare by pointer identity via their existing helpers rather than by value. This
        // also rejects any inherited polymorphic base, which is carried as a subobject field.
        if ((cxxRecordDecl is not null) && (HasVtbl(cxxRecordDecl, out var hasBaseVtbl) || hasBaseVtbl))
        {
            return false;
        }

        // Guard against by-value layout cycles. C/C++ can't nest a complete type in itself by value, but
        // recovering from a malformed/incomplete AST cheaply beats risking unbounded recursion.
        var usr = recordDecl.Handle.Usr.CString;

        if (!string.IsNullOrEmpty(usr) && !visited.Add(usr))
        {
            return false;
        }

        var hasField = false;

        // Base subobjects are emitted as regular record-typed fields (see VisitRecordDecl), so inheritance
        // is just "compare each base subobject, then the declared fields". Each base must itself be
        // field-wise comparable for the same transitivity reason nested fields are.
        if (cxxRecordDecl is not null)
        {
            foreach (var (cxxBaseSpecifier, _) in EnumerateBaseSubobjects(cxxRecordDecl))
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                if (IsBaseExcluded(cxxRecordDecl, baseCxxRecordDecl, cxxBaseSpecifier, out var baseFieldName))
                {
                    continue;
                }

                if (!IsRecordEqualityEligible(baseCxxRecordDecl, baseCxxRecordDecl, fields: null, visited))
                {
                    return false;
                }

                hasField = true;
                fields?.Add((baseFieldName, EqualityFieldKind.Record));
            }
        }

        // Consecutive bitfields collapse into shared backing storage (`_bitfield`, `_bitfield1`, ...); the
        // individual regions are accessor properties over those integers. Comparing the backing fields is
        // both correct and cheaper, so map each backing group to its first region and skip the rest.
        Dictionary<string, string>? bitfieldBacking = null;

        if (recordDecl.Fields.Any(static (fieldDecl) => fieldDecl.IsBitField))
        {
            var bitfieldDescs = GetBitfieldDescs(recordDecl);
            bitfieldBacking = new Dictionary<string, string>(bitfieldDescs.Length);

            for (var i = 0; i < bitfieldDescs.Length; i++)
            {
                var backingName = (bitfieldDescs.Length == 1) ? "_bitfield" : $"_bitfield{i + 1}";
                bitfieldBacking.Add(bitfieldDescs[i].Regions[0].Name, backingName);
            }
        }

        foreach (var fieldDecl in recordDecl.Fields)
        {
            if (fieldDecl.IsBitField)
            {
                // Only the region that starts a backing group contributes the backing field; the others
                // alias the same integer and would double-count.
                if ((bitfieldBacking is not null) && bitfieldBacking.TryGetValue(GetRemappedCursorName(fieldDecl), out var backingName))
                {
                    hasField = true;
                    fields?.Add((backingName, EqualityFieldKind.Value));
                }

                continue;
            }

            var name = GetRemappedCursorName(fieldDecl);

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var canonicalType = fieldDecl.Type.CanonicalType;
            EqualityFieldKind kind;

            if (canonicalType is PointerType)
            {
                kind = EqualityFieldKind.Pointer;
            }
            else if (canonicalType is BuiltinType or EnumType)
            {
                kind = EqualityFieldKind.Value;
            }
            else if (canonicalType is RecordType recordType)
            {
                // Anonymous struct fields land here too: they are promoted to a nested record plus a
                // backing field, so comparing that field compares all promoted members exactly once. An
                // anonymous union nests an IsUnion record and is rejected by the recursion below.
                kind = EqualityFieldKind.Record;

                // The nested struct must itself be field-wise comparable, otherwise Equals(TSelf) would
                // silently defer to ValueType.Equals for it.
                if (!IsRecordEqualityEligible(recordType.Decl, recordType.Decl as CXXRecordDecl, fields: null, visited))
                {
                    return false;
                }
            }
            else
            {
                // Arrays/fixed buffers, function prototypes, member pointers, etc. are not supported yet.
                return false;
            }

            hasField = true;
            fields?.Add((EscapeName(name), kind));
        }

        return hasField;
    }

    private void OutputEqualityMethods(CSharpOutputBuilder outputBuilder, string escapedName, List<(string Name, EqualityFieldKind Kind)> fields)
    {
        outputBuilder.AddUsingDirective("System");

        var readOnly = _config.GenerateCompatibleCode ? "" : "readonly ";

        // public override readonly bool Equals(object? obj) => (obj is TSelf other) && Equals(other);
        outputBuilder.WriteIndented("public override ");
        outputBuilder.Write(readOnly);
        outputBuilder.Write("bool Equals(object? obj) => (obj is ");
        outputBuilder.Write(escapedName);
        outputBuilder.WriteLine(" other) && Equals(other);");
        outputBuilder.NeedsNewline = true;

        // public readonly bool Equals(TSelf other) => (field == other.field) && ...;
        outputBuilder.WriteIndented("public ");
        outputBuilder.Write(readOnly);
        outputBuilder.Write("bool Equals(");
        outputBuilder.Write(escapedName);
        outputBuilder.Write(" other) => ");

        for (var i = 0; i < fields.Count; i++)
        {
            if (i != 0)
            {
                outputBuilder.Write(" && ");
            }

            var (fieldName, fieldKind) = fields[i];

            if (fieldKind == EqualityFieldKind.Record)
            {
                outputBuilder.Write(fieldName);
                outputBuilder.Write(".Equals(other.");
                outputBuilder.Write(fieldName);
                outputBuilder.Write(')');
            }
            else
            {
                outputBuilder.Write('(');
                outputBuilder.Write(fieldName);
                outputBuilder.Write(" == other.");
                outputBuilder.Write(fieldName);
                outputBuilder.Write(')');
            }
        }

        outputBuilder.WriteLine(';');
        outputBuilder.NeedsNewline = true;

        OutputGetHashCode(outputBuilder, readOnly, fields);

        // public static bool operator ==(TSelf left, TSelf right) => left.Equals(right);
        outputBuilder.WriteIndented("public static bool operator ==(");
        outputBuilder.Write(escapedName);
        outputBuilder.Write(" left, ");
        outputBuilder.Write(escapedName);
        outputBuilder.WriteLine(" right) => left.Equals(right);");
        outputBuilder.NeedsNewline = true;

        // public static bool operator !=(TSelf left, TSelf right) => !(left == right);
        outputBuilder.WriteIndented("public static bool operator !=(");
        outputBuilder.Write(escapedName);
        outputBuilder.Write(" left, ");
        outputBuilder.Write(escapedName);
        outputBuilder.WriteLine(" right) => !(left == right);");
        outputBuilder.NeedsNewline = true;
    }

    private static void OutputGetHashCode(CSharpOutputBuilder outputBuilder, string readOnly, List<(string Name, EqualityFieldKind Kind)> fields)
    {
        static string HashOperand((string Name, EqualityFieldKind Kind) field)
        {
            return field.Kind == EqualityFieldKind.Pointer ? $"(nint){field.Name}" : field.Name;
        }

        // HashCode.Combine supports up to eight values; fall back to the incremental builder past that.
        if (fields.Count <= 8)
        {
            outputBuilder.WriteIndented("public override ");
            outputBuilder.Write(readOnly);
            outputBuilder.Write("int GetHashCode() => HashCode.Combine(");
            outputBuilder.Write(string.Join(", ", fields.Select(HashOperand)));
            outputBuilder.WriteLine(");");
            outputBuilder.NeedsNewline = true;
            return;
        }

        outputBuilder.WriteIndented("public override ");
        outputBuilder.Write(readOnly);
        outputBuilder.WriteLine("int GetHashCode()");
        outputBuilder.WriteBlockStart();
        outputBuilder.WriteIndentedLine("var hashCode = new HashCode();");

        foreach (var field in fields)
        {
            outputBuilder.WriteIndented("hashCode.Add(");
            outputBuilder.Write(HashOperand(field));
            outputBuilder.WriteLine(");");
        }

        outputBuilder.WriteIndentedLine("return hashCode.ToHashCode();");
        outputBuilder.WriteBlockEnd();
        outputBuilder.NeedsNewline = true;
    }
}
