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
    private List<(string Name, EqualityFieldKind Kind)>? GetEqualityFields(RecordDecl recordDecl, CXXRecordDecl? cxxRecordDecl, bool hasVtbl, bool hasBaseVtbl)
    {
        if (!_config.GenerateEqualityMethods)
        {
            return null;
        }

        // Unions have overlapping storage so there is no meaningful field-wise comparison, and vtbl/COM
        // types compare by pointer identity via their existing helpers rather than by value.
        if (recordDecl.IsUnion || hasVtbl || hasBaseVtbl)
        {
            return null;
        }

        // Base subobjects are flattened into synthetic fields whose shape is not reflected by
        // recordDecl.Fields, so skip inherited layouts for now.
        if ((cxxRecordDecl is not null) && cxxRecordDecl.Bases.Any())
        {
            return null;
        }

        var fields = new List<(string Name, EqualityFieldKind Kind)>();

        foreach (var fieldDecl in recordDecl.Fields)
        {
            // Bitfields are merged into backing storage and anonymous records are promoted to properties;
            // neither maps cleanly to a single comparable field identifier.
            if (fieldDecl.IsBitField || fieldDecl.IsAnonymousField)
            {
                return null;
            }

            var name = GetRemappedCursorName(fieldDecl);

            if (string.IsNullOrEmpty(name))
            {
                return null;
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
            else if (canonicalType is RecordType)
            {
                kind = EqualityFieldKind.Record;
            }
            else
            {
                // Arrays/fixed buffers, function prototypes, member pointers, etc. are not supported yet.
                return null;
            }

            fields.Add((EscapeName(name), kind));
        }

        return fields.Count != 0 ? fields : null;
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
