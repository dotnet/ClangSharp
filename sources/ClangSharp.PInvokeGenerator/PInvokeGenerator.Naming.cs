// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using ClangSharp.Interop;
using ClangSharp.XML;
using static ClangSharp.Interop.CX_AttrKind;
using static ClangSharp.Interop.CX_CXXAccessSpecifier;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CX_UnaryExprOrTypeTrait;
using static ClangSharp.Interop.CXBinaryOperatorKind;
using static ClangSharp.Interop.CXCallingConv;
using static ClangSharp.Interop.CXDiagnosticSeverity;
using static ClangSharp.Interop.CXEvalResultKind;
using static ClangSharp.Interop.CXTemplateArgumentKind;
using static ClangSharp.Interop.CXTranslationUnit_Flags;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CXUnaryOperatorKind;

namespace ClangSharp;

public sealed partial class PInvokeGenerator
{
    private string GetCursorName(NamedDecl namedDecl)
    {
        if (!_cursorNames.TryGetValue(namedDecl, out var nameString))
        {
            nameString = namedDecl.Name.NormalizePath();
            var name = nameString.AsSpan();

            name = StripCursorNameTagPrefix(name, ref nameString);
            name = StripCursorNameAnonymousQualifier(name, ref nameString);

            if (namedDecl is CXXConstructorDecl cxxConstructorDecl)
            {
                var parent = cxxConstructorDecl.Parent;
                Debug.Assert(parent is not null);

                nameString = GetCursorName(parent);
            }
            else if (namedDecl is CXXDestructorDecl cxxDestructorDecl)
            {
                var parent = cxxDestructorDecl.Parent;
                Debug.Assert(parent is not null);

                nameString = $"~{GetCursorName(parent)}";
            }
            else if (name.IsWhiteSpace() || name.StartsWith('('))
            {
                ResolveAnonymousCursorName(namedDecl, name, ref nameString);
            }

            nameString ??= name.ToString();
            _cursorNames[namedDecl] = nameString;
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(nameString));
        return nameString;
    }

    private static ReadOnlySpan<char> StripCursorNameTagPrefix(ReadOnlySpan<char> name, scoped ref string? nameString)
    {
        if (name.StartsWith("enum ", StringComparison.Ordinal))
        {
            name = name[5..];
            nameString = null;
        }
        else if (name.StartsWith("struct ", StringComparison.Ordinal))
        {
            name = name[7..];
            nameString = null;
        }
        else if (name.StartsWith("union ", StringComparison.Ordinal))
        {
            name = name[6..];
            nameString = null;
        }

        return name;
    }

    private static ReadOnlySpan<char> StripCursorNameAnonymousQualifier(ReadOnlySpan<char> name, scoped ref string? nameString)
    {
        var anonymousNameStartIndex = name.IndexOf("::(", StringComparison.Ordinal);

        if (anonymousNameStartIndex != -1)
        {
            anonymousNameStartIndex += 2;
            name = name[anonymousNameStartIndex..];
            nameString = null;
        }

        return name;
    }

    private void ResolveAnonymousCursorName(NamedDecl namedDecl, ReadOnlySpan<char> name, ref string? nameString)
    {
#if DEBUG
        if (name.StartsWith('('))
        {
            Debug.Assert(name.StartsWith("(anonymous enum at ", StringComparison.Ordinal) ||
                         name.StartsWith("(anonymous struct at ", StringComparison.Ordinal) ||
                         name.StartsWith("(anonymous union at ", StringComparison.Ordinal) ||
                         name.StartsWith("(unnamed enum at ", StringComparison.Ordinal) ||
                         name.StartsWith("(unnamed struct at ", StringComparison.Ordinal) ||
                         name.StartsWith("(unnamed union at ", StringComparison.Ordinal) ||
                         name.StartsWith("(unnamed at ", StringComparison.Ordinal));
            Debug.Assert(name.EndsWith(')'));
        }
#endif

        if (namedDecl is TypeDecl typeDecl)
        {
            nameString = (typeDecl is TagDecl tagDecl) && tagDecl.Handle.IsAnonymous
                       ? GetAnonymousName(tagDecl, tagDecl.TypeForDecl.KindSpelling)
                       : GetTypeName(namedDecl, context: null, type: typeDecl.TypeForDecl, ignoreTransparentStructsWhereRequired: false, isTemplate: false, nativeTypeName: out _);
        }
        else if (namedDecl is ParmVarDecl)
        {
            nameString = "param";
        }
        else if (namedDecl is FieldDecl fieldDecl)
        {
            nameString = GetAnonymousName(fieldDecl, fieldDecl.CursorKindSpelling);
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported anonymous named declaration: '{namedDecl.DeclKindName}'.", namedDecl);
        }
    }

    private string GetCursorQualifiedName(NamedDecl namedDecl, bool truncateParameters = false)
    {
        if (!_cursorQualifiedNames.TryGetValue((namedDecl, truncateParameters), out var qualifiedName))
        {
            var parts = new Stack<NamedDecl>();
            Decl? decl = namedDecl;

            do
            {
                if (decl is NamedDecl parentNamedDecl)
                {
                    parts.Push(parentNamedDecl);
                }

                if ((decl.DeclContext is null) && (decl is CXXMethodDecl cxxMethodDecl))
                {
                    var cxxRecordDecl = cxxMethodDecl.ThisObjectType.AsCXXRecordDecl;
                    Debug.Assert(cxxRecordDecl is not null);
                    decl = cxxRecordDecl;
                }
                else
                {
                    decl = (Decl?)decl.DeclContext;
                }
            }
            while (decl is not null);

            var qualifiedNameBuilder = new StringBuilder();

            var part = parts.Pop();

            while (parts.Count != 0)
            {
                AppendNamedDecl(part, GetCursorName(part), qualifiedNameBuilder);
                _ = qualifiedNameBuilder.Append("::");
                part = parts.Pop();
            }

            AppendNamedDecl(part, GetCursorName(part), qualifiedNameBuilder);

            qualifiedName = qualifiedNameBuilder.ToString();
            _cursorQualifiedNames[(namedDecl, truncateParameters)] = qualifiedName;
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(qualifiedName));
        return qualifiedName;

        void AppendFunctionParameters(CXType functionType, StringBuilder qualifiedName)
        {
            if (truncateParameters)
            {
                return;
            }

            _ = qualifiedName.Append('(');

            if (functionType.NumArgTypes != 0)
            {
                _ = qualifiedName.Append(functionType.GetArgType(0).Spelling);

                for (uint i = 1; i < functionType.NumArgTypes; i++)
                {
                    _ = qualifiedName.Append(',');
                    _ = qualifiedName.Append(' ');
                    _ = qualifiedName.Append(functionType.GetArgType(i).Spelling);
                }
            }

            _ = qualifiedName.Append(')');
            _ = qualifiedName.Append(':');

            _ = qualifiedName.Append(functionType.ResultType.Spelling);

            if (functionType.ExceptionSpecificationType == CXCursor_ExceptionSpecificationKind.CXCursor_ExceptionSpecificationKind_NoThrow)
            {
                _ = qualifiedName.Append(' ');
                _ = qualifiedName.Append("nothrow");
            }
        }

        void AppendNamedDecl(NamedDecl namedDecl, string name, StringBuilder qualifiedName)
        {
            _ = qualifiedName.Append(name);

            if (namedDecl is FunctionDecl functionDecl)
            {
                AppendFunctionParameters(functionDecl.Type.Handle, qualifiedName);
            }
            else if (namedDecl is TemplateDecl templateDecl)
            {
                AppendTemplateParameters(templateDecl, qualifiedName);

                if (namedDecl is FunctionTemplateDecl functionTemplateDecl)
                {
                    AppendFunctionParameters(functionTemplateDecl.Handle.Type, qualifiedName);
                }
            }
            else if (namedDecl is ClassTemplateSpecializationDecl classTemplateSpecializationDecl)
            {
                AppendTemplateArguments(classTemplateSpecializationDecl, qualifiedName);
            }
        }

        void AppendTemplateArgument(TemplateArgument templateArgument, StringBuilder qualifiedName)
        {
            switch (templateArgument.Kind)
            {
                case CXTemplateArgumentKind_Type:
                {
                    // AsType is null when the argument's type maps to an invalid CXType
                    // (an unsupported type the shim can't represent). Fall back to the
                    // same '?' placeholder used for other unsupported argument kinds
                    // rather than dereferencing null.
                    var argType = templateArgument.AsType;
                    _ = qualifiedName.Append((argType is not null) ? argType.AsString : "?");
                    break;
                }

                case CXTemplateArgumentKind_Integral:
                {
                    _ = qualifiedName.Append(templateArgument.AsIntegral);
                    break;
                }

                default:
                {
                    _ = qualifiedName.Append('?');
                    break;
                }
            }
        }

        void AppendTemplateArguments(ClassTemplateSpecializationDecl classTemplateSpecializationDecl, StringBuilder qualifiedName)
        {
            if (truncateParameters)
            {
                return;
            }

            _ = qualifiedName.Append('<');

            var templateArgs = classTemplateSpecializationDecl.TemplateArgs;

            if (templateArgs.Any())
            {
                AppendTemplateArgument(templateArgs[0], qualifiedName);

                for (var i = 1; i < templateArgs.Count; i++)
                {
                    _ = qualifiedName.Append(',');
                    _ = qualifiedName.Append(' ');
                    AppendTemplateArgument(templateArgs[i], qualifiedName);
                }
            }

            _ = qualifiedName.Append('>');
        }

        void AppendTemplateParameters(TemplateDecl templateDecl, StringBuilder qualifiedName)
        {
            if (truncateParameters)
            {
                return;
            }

            _ = qualifiedName.Append('<');

            var templateParameters = templateDecl.TemplateParameters;

            if (templateParameters.Any())
            {
                _ = qualifiedName.Append(templateParameters[0].Name);

                for (var i = 1; i < templateParameters.Count; i++)
                {
                    _ = qualifiedName.Append(',');
                    _ = qualifiedName.Append(' ');
                    _ = qualifiedName.Append(templateParameters[i].Name);
                }
            }

            _ = qualifiedName.Append('>');
        }
    }

    private static Expr GetExprAsWritten(Expr expr, bool removeParens)
    {
        do
        {
            if (expr is ImplicitCastExpr implicitCastExpr)
            {
                expr = implicitCastExpr.SubExprAsWritten;
            }
            else if (removeParens && (expr is ParenExpr parenExpr))
            {
                expr = parenExpr.SubExpr;
            }
            else
            {
                return expr;
            }
        }
        while (true);
    }

    private uint GetOverloadIndex(CXXMethodDecl cxxMethodDeclToMatch)
    {
        if (!_overloadIndices.TryGetValue(cxxMethodDeclToMatch, out var index))
        {
            var parent = cxxMethodDeclToMatch.Parent;
            Debug.Assert(parent is not null);

            index = GetOverloadIndex(cxxMethodDeclToMatch, parent, baseIndex: 0);
            _overloadIndices.Add(cxxMethodDeclToMatch, index);
        }
        return index;

        uint GetOverloadIndex(CXXMethodDecl cxxMethodDeclToMatch, CXXRecordDecl cxxRecordDecl, uint baseIndex)
        {
            var index = baseIndex;

            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                index = GetOverloadIndex(cxxMethodDeclToMatch, baseCxxRecordDecl, index);
            }

            foreach (var cxxMethodDecl in cxxRecordDecl.Methods.OrderBy((cxxmd) => cxxmd.VtblIndex))
            {
                if (IsExcluded(cxxMethodDecl))
                {
                    continue;
                }
                else if (cxxMethodDecl == cxxMethodDeclToMatch)
                {
                    break;
                }
                else if (cxxMethodDecl.Name == cxxMethodDeclToMatch.Name)
                {
                    index++;
                }
            }

            return index;
        }
    }

    private uint GetOverloadCount(CXXMethodDecl cxxMethodDeclToMatch)
    {
        var parent = cxxMethodDeclToMatch.Parent;
        Debug.Assert(parent is not null);

        return GetOverloadIndex(cxxMethodDeclToMatch, parent, baseCount: 0);

        uint GetOverloadIndex(CXXMethodDecl cxxMethodDeclToMatch, CXXRecordDecl cxxRecordDecl, uint baseCount)
        {
            var count = baseCount;

            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                count = GetOverloadIndex(cxxMethodDeclToMatch, baseCxxRecordDecl, count);
            }

            foreach (var cxxMethodDecl in cxxRecordDecl.Methods)
            {
                if (IsExcluded(cxxMethodDecl))
                {
                    continue;
                }
                else if (cxxMethodDecl.Name == cxxMethodDeclToMatch.Name)
                {
                    count++;
                }
            }

            return count;
        }
    }

    private CXXRecordDecl GetRecordDecl(CXXBaseSpecifier cxxBaseSpecifier)
    {
        var baseType = cxxBaseSpecifier.Type;

        if (IsType<RecordType>(cxxBaseSpecifier, baseType, out var recordType))
        {
            return (CXXRecordDecl)recordType.Decl;
        }

        AddDiagnostic(DiagnosticLevel.Error, "Failed to retrieve record type for CXX base specifier. Falling back to referenced type.", cxxBaseSpecifier);
        return (CXXRecordDecl)cxxBaseSpecifier.Referenced;
    }

    private string GetRemappedCursorName(NamedDecl namedDecl) => GetRemappedCursorName(namedDecl, out _, skipUsing: false);

    private string GetRemappedCursorName(NamedDecl namedDecl, out string nativeTypeName, bool skipUsing)
    {
        nativeTypeName = GetCursorQualifiedName(namedDecl);

        // A `--remap-type` / `--remap-field` entry takes precedence over the general `--remap` so a
        // type and a field that share a name (legal in C, but not C#) can be disambiguated. These
        // are only consulted here, on the decl's own name, so that resolving the *type* referenced
        // by a field (which reuses the field cursor) doesn't pick up a `--remap-field` entry.
        var kindSpecificRemappedNames = namedDecl switch {
            FieldDecl => _config._remappedFieldNames,
            TypeDecl => _config._remappedTypeNames,
            _ => null,
        };

        if ((kindSpecificRemappedNames is not null) && (kindSpecificRemappedNames.Count != 0))
        {
            var kindSpecificLookup = kindSpecificRemappedNames.GetAlternateLookup<ReadOnlySpan<char>>();

            if (kindSpecificLookup.TryGetValue(GetCursorName(namedDecl), out var kindSpecificRemappedName))
            {
                return AddUsingDirectiveIfNeeded(_outputBuilder, kindSpecificRemappedName, skipUsing);
            }
        }

        var name = nativeTypeName;
        var remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true, out var wasRemapped, skipUsing);

        if (wasRemapped)
        {
            return remappedName;
        }

        name = GetCursorQualifiedName(namedDecl, truncateParameters: true);
        remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true, out wasRemapped, skipUsing);

        if (wasRemapped)
        {
            return remappedName;
        }

        name = GetCursorName(namedDecl);
        remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true, out wasRemapped, skipUsing);

        if (wasRemapped)
        {
            return remappedName;
        }

        if (namedDecl is CXXConstructorDecl cxxConstructorDecl)
        {
            var parent = cxxConstructorDecl.Parent;
            Debug.Assert(parent is not null);
            remappedName = GetRemappedCursorName(parent);
        }
        else if (namedDecl is CXXDestructorDecl)
        {
            remappedName = "Dispose";
        }
        else if ((namedDecl is FieldDecl fieldDecl) && name.StartsWith(AnonymousFieldDeclPrefix, StringComparison.Ordinal))
        {
            if (fieldDecl.Type.AsCXXRecordDecl?.IsAnonymousStructOrUnion == true)
            {
                // For fields of anonymous types, use the name of the type but clean off the type
                // kind tag at the end.
                var typeName = GetRemappedNameForAnonymousRecord(fieldDecl.Type.AsCXXRecordDecl);
                var tagIndex = typeName.LastIndexOf(AnonymousTypeKindTag, StringComparison.Ordinal);
                Debug.Assert(typeName[0] == '_');
                Debug.Assert(tagIndex >= 0);
                remappedName = typeName[1..tagIndex];
            }
            else
            {
                remappedName = "Anonymous";

                var parent = fieldDecl.Parent;
                Debug.Assert(parent is not null);

                if (parent.AnonymousFields.Count > 1)
                {
                    var index = parent.AnonymousFields.IndexOf(fieldDecl) + 1;
                    remappedName += index.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        else if ((namedDecl is RecordDecl recordDecl) && IsAnonymousRecord(name))
        {
            remappedName = GetRemappedNameForAnonymousRecord(recordDecl);
        }
        else if ((namedDecl is RecordDecl clashingRecordDecl) && TryDeclashRecordName(clashingRecordDecl, remappedName, out var declashedName))
        {
            remappedName = declashedName;
        }

        return remappedName;
    }

    private string ApplyTagTypeNameOverrides(TagType tagType, string leafName)
    {
        // Keep a `--remap-type` override or a de-clashed nested record name (see
        // GetRemappedCursorName) consistent between the type declaration and any reference to it,
        // so both resolve to the same C# type.

        if (_config._remappedTypeNames.Count != 0)
        {
            var remappedTypeNamesLookup = _config._remappedTypeNames.GetAlternateLookup<ReadOnlySpan<char>>();

            if (remappedTypeNamesLookup.TryGetValue(leafName, out var remappedTypeName))
            {
                return remappedTypeName;
            }
        }

        if ((tagType.Decl is RecordDecl recordDecl) && TryDeclashRecordName(recordDecl, leafName, out var declashedName))
        {
            return declashedName;
        }

        return leafName;
    }

    private bool TryDeclashRecordName(RecordDecl recordDecl, string name, out string declashedName)
    {
        declashedName = name;

        // A named nested record is legal in C even when a sibling field shares its name, but the
        // two would map to a nested type and a field of the same name in C#, which is a CS0102
        // clash. Rename the type using the anonymous-record naming so both the declaration and any
        // field type references resolve consistently through this method.
        //
        // The lexical parent is used (not the semantic one) since C promotes a nested record to the
        // enclosing scope, yet it is still emitted nested based on where it lexically appears.

        if (recordDecl.LexicalDeclContext is not RecordDecl parentRecordDecl)
        {
            return false;
        }

        var hasClash = false;

        foreach (var fieldDecl in parentRecordDecl.Fields)
        {
            if (GetRemappedCursorName(fieldDecl) == name)
            {
                hasClash = true;
                break;
            }
        }

        if (!hasClash)
        {
            return false;
        }

        declashedName = $"_{name}{AnonymousTypeKindTag}{(recordDecl.IsUnion ? "Union" : "Struct")}";

        if (_declashedRecordNames.Add(recordDecl))
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Renamed nested type '{name}' to '{declashedName}' to avoid a name clash with a field of the same name in '{GetRemappedCursorName(parentRecordDecl)}'. Use '--remap-type {name}=NewName' or '--remap-field {name}=NewName' to control the naming explicitly.", recordDecl);
        }

        return true;
    }

    private static int GetAnonymousRecordIndex(RecordDecl recordDecl, RecordDecl parentRecordDecl)
    {
        var index = -1;
        var parentAnonRecordCount = parentRecordDecl.AnonymousRecords.Count;

        if (parentAnonRecordCount != 0)
        {
            index = parentRecordDecl.AnonymousRecords.IndexOf(recordDecl);

            if (index != -1)
            {
                if (parentAnonRecordCount > 1)
                {
                    index++;
                }

                if (parentRecordDecl.Parent is RecordDecl grandparentRecordDecl)
                {
                    var parentIndex = GetAnonymousRecordIndex(parentRecordDecl, grandparentRecordDecl);

                    // We can't have the nested anonymous record have the same name as the parent
                    // so skip that index and just go one higher instead. This could still conflict
                    // with another anonymous record at a different level, but that is less likely
                    // and will still be unambiguous in total.

                    if ((parentIndex == index) || ((parentIndex > 0) && (index > parentIndex)))
                    {
                        if (recordDecl.IsUnion == parentRecordDecl.IsUnion)
                        {
                            index++;
                        }
                    }
                }
            }
        }

        return index;
    }

    private string GetRemappedNameForAnonymousRecord(RecordDecl recordDecl)
    {
        if (recordDecl.Parent is RecordDecl parentRecordDecl)
        {
            var remappedNameBuilder = new StringBuilder();
            var matchingField = null as FieldDecl;

            if (!recordDecl.IsAnonymousStructOrUnion)
            {
                matchingField = parentRecordDecl.Fields.Where((fieldDecl) => {
                    var fieldType = fieldDecl.Type.CanonicalType;

                    if (fieldType is ArrayType arrayType)
                    {
                        fieldType = arrayType.ElementType.CanonicalType;
                    }

                    return fieldType == recordDecl.TypeForDecl.CanonicalType;
                }).FirstOrDefault();
            }

            if ((matchingField is not null) && !matchingField.IsAnonymousField)
            {
                _ = remappedNameBuilder.Append('_');
                _ = remappedNameBuilder.Append(GetRemappedCursorName(matchingField));
            }
            else
            {
                _ = remappedNameBuilder.Append("_Anonymous");

                var index = GetAnonymousRecordIndex(recordDecl, parentRecordDecl);

                if (index != 0)
                {
                    _ = remappedNameBuilder.Append(index);
                }
            }

            // Add the type kind tag.
            _ = remappedNameBuilder.Append(AnonymousTypeKindTag);
            _ = remappedNameBuilder.Append(recordDecl.IsUnion ? "Union" : "Struct");
            return remappedNameBuilder.ToString();
        }
        else
        {
            return $"_Anonymous{AnonymousTypeKindTag}{(recordDecl.IsUnion ? "Union" : "Struct")}";
        }
    }

    private string GetRemappedName(string name, Cursor? cursor, bool tryRemapOperatorName, out bool wasRemapped, bool skipUsing = false)
        => GetRemappedName(name, cursor, tryRemapOperatorName, out wasRemapped, skipUsing, skipUsingIfNotRemapped: skipUsing);

    private string GetRemappedName(string name, Cursor? cursor, bool tryRemapOperatorName, out bool wasRemapped, bool skipUsing, bool skipUsingIfNotRemapped)
    {
        var remappedNamesLookup = _config._remappedNames.GetAlternateLookup<ReadOnlySpan<char>>();

        if (remappedNamesLookup.TryGetValue(name, out var remappedName))
        {
            wasRemapped = true;
            _ = _usedRemappings.Add(name);
            return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsing);
        }

        if (name.StartsWith("const ", StringComparison.Ordinal))
        {
            var tmpName = name.AsSpan()[6..];

            if (remappedNamesLookup.TryGetValue(tmpName, out remappedName))
            {

                wasRemapped = true;
                _ = _usedRemappings.Add(tmpName.ToString());
                return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsing);
            }
        }

        remappedName = name;

        if ((cursor is FunctionDecl functionDecl) && tryRemapOperatorName && TryRemapOperatorName(ref remappedName, functionDecl))
        {
            wasRemapped = true;
            // We don't track remapped operators in _usedRemappings
            return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsing);
        }

        if ((cursor is CXXBaseSpecifier cxxBaseSpecifier) && remappedName.StartsWith(AnonymousBasePrefix, StringComparison.Ordinal))
        {
            Debug.Assert(_cxxRecordDeclContext is not null);
            remappedName = "Base";

            if (_cxxRecordDeclContext.Bases.Count > 1)
            {
                var index = _cxxRecordDeclContext.Bases.IndexOf(cxxBaseSpecifier) + 1;
                remappedName += index.ToString(CultureInfo.InvariantCulture);
            }

            wasRemapped = true;
            return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsing);
        }

        wasRemapped = false;
        return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsingIfNotRemapped);
    }

    private string AddUsingDirectiveIfNeeded(IOutputBuilder? outputBuilder, string remappedName, bool skipUsing)
    {
        if (!skipUsing)
        {
            if (NeedsSystemSupportRegex().IsMatch(remappedName))
            {
                outputBuilder?.EmitSystemSupport();
            }

            var namespaceName = GetNamespace(remappedName);
            AddUsingDirective(outputBuilder, namespaceName);
        }

        return remappedName;
    }

    private string GetRemappedTypeName(Cursor? cursor, Cursor? context, Type type, out string nativeTypeName, bool skipUsing = false, bool ignoreTransparentStructsWhereRequired = false, bool isTemplate = false)
    {
        var name = GetTypeName(cursor, context, type, ignoreTransparentStructsWhereRequired, isTemplate: isTemplate, nativeTypeName: out nativeTypeName);

        var nameToCheck = nativeTypeName;
        var remappedName = GetRemappedName(nameToCheck, cursor, tryRemapOperatorName: false, out var wasRemapped, skipUsing, skipUsingIfNotRemapped: true);

        if (!wasRemapped)
        {
            nameToCheck = name;
            remappedName = GetRemappedName(nameToCheck, cursor, tryRemapOperatorName: false, out wasRemapped, skipUsing);

            if (!wasRemapped)
            {
                if (IsTypeConstantOrIncompleteArray(cursor, type, out var arrayType) && IsType<RecordType>(cursor, arrayType.ElementType))
                {
                    type = arrayType.ElementType;
                }

                if (IsType<RecordType>(cursor, type, out var recordType) && IsAnonymousRecord(remappedName))
                {
                    var recordDecl = recordType.Decl;
                    remappedName = GetRemappedNameForAnonymousRecord(recordDecl);
                }
                else if (IsType<EnumType>(cursor, type, out var enumType) && IsAnonymousEnum(remappedName))
                {
                    remappedName = GetRemappedTypeName(enumType.Decl, context: null, enumType.Decl.IntegerType, out _, skipUsing);
                }
                else if (cursor is EnumDecl enumDecl)
                {
                    // Even though some types have entries with names like *_FORCE_DWORD or *_FORCE_UINT
                    // MSVC and Clang both still treat this as "signed" values and thus we don't want
                    // to specially handle it as uint, as that can break ABI handling on some platforms.

                    WithType(enumDecl, ref remappedName, ref nativeTypeName);
                }
            }
        }

        if (string.IsNullOrWhiteSpace(nativeTypeName))
        {
            // When we have an empty native type name, it means the original
            // name is the same as the native type name and no adjustments
            // were made. In order to ensure things are correctly preserved
            // we need to ensure its propagated back here so the below comparison
            // works and we don't end up comparing "empty" vs "remapped"
            nativeTypeName = name;
        }

        if (IsNativeTypeNameEquivalent(nativeTypeName, remappedName))
        {
            // Empty the native type name if its equivalent to the new name
            nativeTypeName = string.Empty;
        }

        if (remappedName.IndexOf('[', StringComparison.Ordinal) is int bracketIndex and >= 0 &&
            (bracketIndex + 1) < remappedName.Length && char.IsAsciiDigit(remappedName[bracketIndex + 1]))
        {
            // A type remapped to a fixed-size-buffer shape, such as `sbyte[8]`, is not a
            // legal C# type in an unmanaged signature. Lower it to the equivalent pointer,
            // matching how C decays an array to a pointer at the ABI boundary. This also
            // ensures the containing member is correctly detected as unsafe via the `*`.
            // A managed array such as `int[]` has an empty subscript and is left untouched.

            var rank = remappedName.AsSpan().Count('[');
            remappedName = string.Concat(remappedName.AsSpan(0, bracketIndex), new string('*', rank));
        }

        return remappedName;
    }
}

