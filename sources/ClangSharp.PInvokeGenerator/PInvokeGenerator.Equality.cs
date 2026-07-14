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
        FunctionPointer,
        Record,
        Array,
    }

    // A single operand of a field-wise comparison. Element is the managed element type name for `Array`
    // (an InlineArray fixed buffer compared via ReadOnlySpan.SequenceEqual) and null otherwise.
    private readonly record struct EqualityField(string Name, EqualityFieldKind Kind, string? Element = null);

    // Collects the instance fields that a field-wise equality implementation would compare, or returns
    // null when the record is not a safe candidate for `generate-equality-methods`. Element-wise equality
    // is only ever opt-in because it is not universally valid (padding, exported comparison functions),
    // so anything that cannot be compared field-for-field here is left untouched rather than guessed at.
    private List<EqualityField>? GetEqualityFields(RecordDecl recordDecl, CXXRecordDecl? cxxRecordDecl)
    {
        if (!_config.GenerateEqualityMethods)
        {
            return null;
        }

        var fields = new List<EqualityField>();

        // Eligibility is transitive: the emitted Equals(TSelf) calls each nested struct's own Equals, so
        // if any record anywhere in the layout can't be compared field-for-field the whole thing falls
        // back to reflection-based ValueType.Equals (padding-sensitive and slow), which is exactly what
        // this feature exists to avoid. Reject the outer type unless the full transitive layout is safe.
        return IsRecordEqualityEligible(recordDecl, cxxRecordDecl, fields, []) && (fields.Count != 0) ? fields : null;
    }

    private bool IsRecordEqualityEligible(RecordDecl recordDecl, CXXRecordDecl? cxxRecordDecl, List<EqualityField>? fields, HashSet<string> visited)
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
                fields?.Add(new EqualityField(baseFieldName, EqualityFieldKind.Record));
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
                    fields?.Add(new EqualityField(backingName, EqualityFieldKind.Value));
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
            string? elementTypeName = null;

            if (canonicalType is PointerType pointerType)
            {
                // A function-pointer field is emitted as `delegate*unmanaged<...>`, which can't be
                // compared with `==` (CS8909); route it through a `void*` cast so identity comparison
                // stays warning-free. When fnptr codegen is disabled the field is a plain `nint`, so the
                // ordinary pointer path already compares cleanly.
                kind = (!_config.ExcludeFnptrCodegen && pointerType.PointeeType.CanonicalType is FunctionType)
                    ? EqualityFieldKind.FunctionPointer
                    : EqualityFieldKind.Pointer;
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
            else if (IsTypeConstantOrIncompleteArray(fieldDecl, out var arrayType))
            {
                // Fixed buffers are only comparable via a span when they are emitted as an InlineArray;
                // the compatible-mode C# `fixed` buffer would need unsafe pointer loops instead.
                if (_config.GenerateCompatibleCode || (arrayType is not ConstantArrayType constantArrayType))
                {
                    return false;
                }

                var totalSize = constantArrayType.Size;
                var elementType = arrayType.ElementType;

                while (IsTypeConstantOrIncompleteArray(fieldDecl, elementType, out var subArrayType))
                {
                    if (subArrayType is not ConstantArrayType subConstantArrayType)
                    {
                        return false;
                    }

                    totalSize *= subConstantArrayType.Size;
                    elementType = subArrayType.ElementType;
                }

                // A single-element or pointer/record-element buffer isn't emitted as an InlineArray, so it
                // doesn't expose the span this relies on. Restrict to primitive/enum elements, which are
                // always bitwise-comparable via ReadOnlySpan.SequenceEqual.
                if ((totalSize <= 1) || (elementType.CanonicalType is not (BuiltinType or EnumType)))
                {
                    return false;
                }

                kind = EqualityFieldKind.Array;
                elementTypeName = GetRemappedTypeName(fieldDecl, context: null, elementType, out _);
            }
            else
            {
                // Function prototypes, member pointers, etc. are not supported.
                return false;
            }

            hasField = true;
            fields?.Add(new EqualityField(EscapeName(name), kind, elementTypeName));
        }

        return hasField;
    }

    private void OutputEqualityMethods(CSharpOutputBuilder outputBuilder, string escapedName, List<EqualityField> fields)
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

            var field = fields[i];

            if (field.Kind == EqualityFieldKind.Record)
            {
                outputBuilder.Write(field.Name);
                outputBuilder.Write(".Equals(other.");
                outputBuilder.Write(field.Name);
                outputBuilder.Write(')');
            }
            else if (field.Kind == EqualityFieldKind.FunctionPointer)
            {
                // ((void*)Field == (void*)other.Field) -- avoids CS8909 on function-pointer comparison.
                outputBuilder.Write("((void*)");
                outputBuilder.Write(field.Name);
                outputBuilder.Write(" == (void*)other.");
                outputBuilder.Write(field.Name);
                outputBuilder.Write(')');
            }
            else if (field.Kind == EqualityFieldKind.Array)
            {
                // ((ReadOnlySpan<TElem>)Field).SequenceEqual(other.Field)
                outputBuilder.Write("((ReadOnlySpan<");
                outputBuilder.Write(field.Element!);
                outputBuilder.Write(">)");
                outputBuilder.Write(field.Name);
                outputBuilder.Write(").SequenceEqual(other.");
                outputBuilder.Write(field.Name);
                outputBuilder.Write(')');
            }
            else
            {
                outputBuilder.Write('(');
                outputBuilder.Write(field.Name);
                outputBuilder.Write(" == other.");
                outputBuilder.Write(field.Name);
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

    private static void OutputGetHashCode(CSharpOutputBuilder outputBuilder, string readOnly, List<EqualityField> fields)
    {
        static string HashOperand(EqualityField field)
        {
            return field.Kind switch {
                // Raw pointers aren't valid HashCode.Combine type args; a function pointer additionally
                // needs the `void*` hop before it can reach `nint`.
                EqualityFieldKind.Pointer => $"(nint){field.Name}",
                EqualityFieldKind.FunctionPointer => $"(nint)(void*){field.Name}",
                _ => field.Name,
            };
        }

        // An InlineArray can't be fed to HashCode.Combine/Add directly, so any array field forces the
        // incremental builder path where each element is folded in individually.
        var hasArray = fields.Any(static (field) => field.Kind == EqualityFieldKind.Array);

        // HashCode.Combine supports up to eight values; fall back to the incremental builder past that.
        if (!hasArray && (fields.Count <= 8))
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
            if (field.Kind == EqualityFieldKind.Array)
            {
                outputBuilder.WriteIndented("foreach (var element in (ReadOnlySpan<");
                outputBuilder.Write(field.Element!);
                outputBuilder.Write(">)");
                outputBuilder.Write(field.Name);
                outputBuilder.WriteLine(')');
                outputBuilder.WriteBlockStart();
                outputBuilder.WriteIndentedLine("hashCode.Add(element);");
                outputBuilder.WriteBlockEnd();
                continue;
            }

            outputBuilder.WriteIndented("hashCode.Add(");
            outputBuilder.Write(HashOperand(field));
            outputBuilder.WriteLine(");");
        }

        outputBuilder.WriteIndentedLine("return hashCode.ToHashCode();");
        outputBuilder.WriteBlockEnd();
        outputBuilder.NeedsNewline = true;
    }
}
