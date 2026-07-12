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
    private static bool IsEnumOperator(FunctionDecl functionDecl, string name)
    {
        if (name.StartsWith("operator", StringComparison.Ordinal) && ((functionDecl.Parameters.Count == 1) || (functionDecl.Parameters.Count == 2)))
        {
            var parmVarDecl1 = functionDecl.Parameters[0];
            var parmVarDecl1Type = parmVarDecl1.Type;

            if (IsType<PointerType>(parmVarDecl1, parmVarDecl1Type, out var pointerType1))
            {
                parmVarDecl1Type = pointerType1.PointeeType;
            }
            else if (IsType<ReferenceType>(parmVarDecl1, parmVarDecl1Type, out var referenceType1))
            {
                parmVarDecl1Type = referenceType1.PointeeType;
            }

            if (functionDecl.Parameters.Count == 1)
            {
                return IsType<EnumType>(parmVarDecl1);
            }

            var parmVarDecl2 = functionDecl.Parameters[1];
            var parmVarDecl2Type = parmVarDecl2.Type;

            if (IsType<PointerType>(parmVarDecl2, parmVarDecl2Type, out var pointerType2))
            {
                parmVarDecl2Type = pointerType2.PointeeType;
            }
            else if (IsType<ReferenceType>(parmVarDecl2, parmVarDecl2Type, out var referenceType2))
            {
                parmVarDecl2Type = referenceType2.PointeeType;
            }

            if ((parmVarDecl1Type.CanonicalType == parmVarDecl2Type.CanonicalType) && IsType<EnumType>(parmVarDecl2))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsExcluded(Cursor cursor) => IsExcluded(cursor, out _);

    private bool IsExcluded(Cursor cursor, out bool isExcludedByConflictingDefinition)
    {
        if (!_isExcluded.TryGetValue(cursor, out var isExcludedValue))
        {
            isExcludedValue |= (!IsAlwaysIncluded(cursor) && (IsExcludedByConfig(cursor) || IsExcludedByFile(cursor) || IsExcludedByName(cursor, ref isExcludedValue) || IsExcludedByAttributes(cursor))) ? 0b01u : 0b00u;
            _isExcluded.Add(cursor, isExcludedValue);
        }
        isExcludedByConflictingDefinition = (isExcludedValue & 0b10) != 0;
        return (isExcludedValue & 0b01) != 0;

        bool IsAlwaysIncluded(Cursor cursor)
        {
            return (cursor is TranslationUnitDecl) || (cursor is LinkageSpecDecl) || (cursor is NamespaceDecl) || ((cursor is VarDecl varDecl) && varDecl.Name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal));
        }

        bool IsExcludedByConfig(Cursor cursor)
        {
            return (_config.ExcludeFunctionsWithBody && (cursor is FunctionDecl functionDecl) && functionDecl.HasBody)
                || (!_config.GenerateTemplateBindings && ((cursor is TemplateDecl) || (cursor is ClassTemplateSpecializationDecl)));
        }

        bool IsExcludedByFile(Cursor cursor)
        {
            if (_outputBuilder != null)
            {
                // We don't want to exclude by file if we already have an active output builder as we
                // are likely processing members of an already included type but those members may
                // indirectly exist or be defined in a non-traversed file.
                return false;
            }

            var declLocation = cursor.Location;
            declLocation.GetFileLocation(out var file, out var line, out var column, out _);

            if (IsIncludedFileOrLocation(cursor, file, declLocation))
            {
                return false;
            }

            // It is not uncommon for some declarations to be done using macros, which are themselves
            // defined in an imported header file. We want to also check if the expansion location is
            // in the main file to catch these cases and ensure we still generate bindings for them.

            declLocation.GetExpansionLocation(out var expansionFile, out var expansionLine, out var expansionColumn, out _);

            if ((expansionFile == file) && (expansionLine == line) && (expansionColumn == column) && _config.TraversalNames.Count != 0)
            {
                // clang_getLocation is a very expensive call, so exit early if the expansion file is the same
                // However, if we are not explicitly specifying traversal names, its possible the expansion location
                // is the same, but IsMainFile is now marked as true, in which case we can't exit early.

                return true;
            }

            var expansionLocation = cursor.TranslationUnit.Handle.GetLocation(expansionFile, expansionLine, expansionColumn);

            return !IsIncludedFileOrLocation(cursor, file, expansionLocation);
        }

        bool IsExcludedByName(Cursor cursor, ref uint isExcludedValue)
        {
            var isExcludedByConfigOption = false;
            var qualifiedNameWithoutParameters = "";

            string qualifiedName;
            string name;
            string kind;

            if (cursor is NamedDecl namedDecl)
            {
                // We get the non-remapped name for the purpose of exclusion checks to ensure that users
                // can remove no-definition declarations in favor of remapped anonymous declarations.

                qualifiedName = GetCursorQualifiedName(namedDecl);

                if (namedDecl is FunctionDecl)
                {
                    qualifiedNameWithoutParameters = GetCursorQualifiedName(namedDecl, truncateParameters: true);
                }

                name = GetCursorName(namedDecl);
                kind = $"{namedDecl.DeclKindName} declaration";

                if ((namedDecl is TagDecl tagDecl) && (tagDecl.Definition != tagDecl) && (tagDecl.Definition != null))
                {
                    // We don't want to generate bindings for anything
                    // that is not itself a definition and that has a
                    // definition that can be resolved. This ensures we
                    // still generate bindings for things which are used
                    // as opaque handles, but which aren't ever defined.

                    if (_config.LogExclusions)
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"Excluded {kind} '{qualifiedName}' by as it is not a definition.");
                    }
                    return true;
                }
            }
            else if (cursor is MacroDefinitionRecord macroDefinitionRecord)
            {
                qualifiedName = macroDefinitionRecord.Name;
                name = macroDefinitionRecord.Name;
                kind = macroDefinitionRecord.CursorKindSpelling;
            }
            else
            {
                return false;
            }

            if (qualifiedName.Contains("ClangSharpMacro_", StringComparison.Ordinal))
            {
                qualifiedName = qualifiedName.Replace("ClangSharpMacro_", "", StringComparison.Ordinal);
            }

            if (name.Contains("ClangSharpMacro_", StringComparison.Ordinal))
            {
                name = name.Replace("ClangSharpMacro_", "", StringComparison.Ordinal);
            }

            if (cursor is RecordDecl recordDecl)
            {
                if (_config.ExcludeEmptyRecords && IsEmptyRecord(recordDecl))
                {
                    isExcludedByConfigOption = true;
                }
            }
            else if (cursor is FunctionDecl functionDecl)
            {
                if (_config.ExcludeComProxies && IsComProxy(functionDecl, name))
                {
                    isExcludedByConfigOption = true;
                }
                else if (_config.ExcludeEnumOperators && IsEnumOperator(functionDecl, name))
                {
                    isExcludedByConfigOption = true;
                }
                else if (functionDecl is CXXMethodDecl cxxMethodDecl)
                {
                    var parent = cxxMethodDecl.Parent;
                    Debug.Assert(parent is not null);

                    if (IsConflictingMethodDecl(cxxMethodDecl, parent))
                    {
                        isExcludedValue |= 0b10;
                    }
                }

                if (_config.GenerateDisableRuntimeMarshalling && functionDecl.IsVariadic)
                {
                    isExcludedByConfigOption = true;
                }
            }

            if (_config.ExcludedNames.Contains(qualifiedName))
            {
                if (_config.LogExclusions)
                {
                    var message = $"Excluded {kind} '{qualifiedName}' by exact match";

                    if (isExcludedByConfigOption)
                    {
                        message += "; Exclusion is unnecessary due to a config option";
                    }
                    else if ((isExcludedValue & 0b10) != 0)
                    {
                        message += "; Exclusion is unnecessary due to a conflicting definition";
                    }

                    AddDiagnostic(DiagnosticLevel.Info, message);
                }
                return true;
            }

            if (_config.ExcludedNames.Contains(qualifiedNameWithoutParameters) || _config.ExcludedNames.Contains(name))
            {
                if (_config.LogExclusions)
                {
                    var message = $"Excluded {kind} '{qualifiedName}' by partial match against {name}";

                    if (isExcludedByConfigOption)
                    {
                        message += "; Exclusion is unnecessary due to a config option";
                    }
                    else if ((isExcludedValue & 0b10) != 0)
                    {
                        message += "; Exclusion is unnecessary due to a conflicting definition";
                    }

                    AddDiagnostic(DiagnosticLevel.Info, message);
                }
                return true;
            }

            if (isExcludedByConfigOption)
            {
                if (_config.LogExclusions)
                {
                    AddDiagnostic(DiagnosticLevel.Info, $"Excluded {kind} '{qualifiedName}' by config option");
                }
                return true;
            }

            if (_config.IncludedNames.Count != 0 && !_config.IncludedNames.Contains(qualifiedName)
                                                 && !_config.IncludedNames.Contains(qualifiedNameWithoutParameters)
                                                 && !_config.IncludedNames.Contains(name))
            {
                var semanticParentCursor = cursor.SemanticParentCursor;

                if ((semanticParentCursor is null) || IsExcluded(semanticParentCursor) || IsAlwaysIncluded(semanticParentCursor))
                {
                    if (_config.LogExclusions)
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"Excluded {kind} '{qualifiedName}' as it was not in the include list");
                    }
                    return true;
                }
            }

            if ((isExcludedValue & 0b10) != 0)
            {
                if (_config.LogExclusions)
                {
                    AddDiagnostic(DiagnosticLevel.Info, $"Excluded {kind} '{qualifiedName}' by conflicting definition");
                }
                return true;
            }

            return false;
        }

        bool IsIncludedFileOrLocation(Cursor cursor, CXFile file, CXSourceLocation location)
        {
            // Use case insensitive comparison on Windows
            var equalityComparer = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            // Normalize paths to be '/' for comparison
            var fileName = file.Name.ToString().NormalizePath();

            if (_visitedFiles.Add(fileName) && _config.LogVisitedFiles)
            {
                AddDiagnostic(DiagnosticLevel.Info, $"Visiting {fileName}");
            }

            if (_config.TraversalNames.Contains(fileName, equalityComparer))
            {
                return true;
            }
            else if (_config.TraversalNames.Contains(fileName.NormalizeFullPath(), equalityComparer))
            {
                return true;
            }
            else if (_config.TraversalNames.Count == 0 && location.IsFromMainFile)
            {
                return true;
            }

            return false;
        }

        bool IsComProxy(FunctionDecl functionDecl, string name)
        {
            var parmVarDecl = null as ParmVarDecl;

            if (name.EndsWith("_UserFree", StringComparison.Ordinal) || name.EndsWith("_UserFree64", StringComparison.Ordinal) ||
                name.EndsWith("_UserMarshal", StringComparison.Ordinal) || name.EndsWith("_UserMarshal64", StringComparison.Ordinal) ||
                name.EndsWith("_UserSize", StringComparison.Ordinal) || name.EndsWith("_UserSize64", StringComparison.Ordinal) ||
                name.EndsWith("_UserUnmarshal", StringComparison.Ordinal) || name.EndsWith("_UserUnmarshal64", StringComparison.Ordinal))
            {
                var parameters = functionDecl.Parameters;
                parmVarDecl = (parameters.Count != 0) ? parameters[^1] : null;
            }
            else if (name.EndsWith("_Proxy", StringComparison.Ordinal) || name.EndsWith("_Stub", StringComparison.Ordinal))
            {
                var parameters = functionDecl.Parameters;
                parmVarDecl = (parameters.Count != 0) ? parameters[0] : null;
            }

            if ((parmVarDecl is not null) && IsType<PointerType>(parmVarDecl, out var pointerType))
            {
                var typeName = GetTypeName(parmVarDecl, context: null, type: pointerType.PointeeType, ignoreTransparentStructsWhereRequired: false, isTemplate: false, nativeTypeName: out var nativeTypeName);
                return name.StartsWith($"{nativeTypeName}_", StringComparison.Ordinal) || name.StartsWith($"{typeName}_", StringComparison.Ordinal) || typeName.Equals("IRpcStubBuffer", StringComparison.Ordinal);
            }
            return false;
        }

        bool IsConflictingMethodDecl(CXXMethodDecl cxxMethodDeclToMatch, CXXRecordDecl cxxRecordDecl)
        {
            var cxxMethodDeclToMatchName = GetRemappedCursorName(cxxMethodDeclToMatch);
            var foundCxxMethodDeclToMatch = false;

            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                if (ContainsConflictingMethodDecl(cxxMethodDeclToMatch, cxxRecordDecl, baseCxxRecordDecl, cxxMethodDeclToMatchName, ref foundCxxMethodDeclToMatch))
                {
                    return true;
                }
            }

            return ContainsConflictingMethodDecl(cxxMethodDeclToMatch, cxxRecordDecl, cxxRecordDecl, cxxMethodDeclToMatchName, ref foundCxxMethodDeclToMatch);

            bool ContainsConflictingMethodDecl(CXXMethodDecl cxxMethodDeclToMatch, CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, string cxxMethodDeclToMatchName, ref bool foundCxxMethodDeclToMatch)
            {
                var cxxMethodDecls = cxxRecordDecl.Methods;

                if (cxxMethodDecls.Count != 0)
                {
                    foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                    {
                        if (IsConflictingMethodDecl(cxxMethodDeclToMatch, cxxMethodDecl, rootCxxRecordDecl, cxxRecordDecl, cxxMethodDeclToMatchName, ref foundCxxMethodDeclToMatch))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            bool IsConflictingMethodDecl(CXXMethodDecl cxxMethodDeclToMatch, CXXMethodDecl cxxMethodDecl, CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, string cxxMethodDeclToMatchName, ref bool foundCxxMethodDeclToMatch)
            {
                var methodName = GetRemappedCursorName(cxxMethodDecl);

                if (cxxMethodDeclToMatchName != methodName)
                {
                    return false;
                }

                if (cxxMethodDecl == cxxMethodDeclToMatch)
                {
                    foundCxxMethodDeclToMatch = true;
                    return false;
                }

                if (cxxMethodDecl.Parameters.Count != cxxMethodDeclToMatch.Parameters.Count)
                {
                    return false;
                }

                var allMatch = true;

                for (var n = 0; n < cxxMethodDeclToMatch.Parameters.Count; n++)
                {
                    var parameterTypeToMatch = cxxMethodDeclToMatch.Parameters[n].Type;
                    var parameterType = cxxMethodDecl.Parameters[n].Type;

                    if (parameterType.CanonicalType == parameterTypeToMatch.CanonicalType)
                    {
                        continue;
                    }

                    if (IsType<PointerType>(cursor, parameterTypeToMatch, out var pointerTypeToMatch) &&
                        IsType<ReferenceType>(cursor, parameterType, out var referenceType) &&
                        (referenceType.PointeeType.CanonicalType == pointerTypeToMatch.PointeeType.CanonicalType))
                    {
                        continue;
                    }

                    if (IsType<ReferenceType>(cursor, parameterTypeToMatch, out var referenceTypeToMatch) &&
                        IsType<PointerType>(cursor, parameterType, out var pointerType) &&
                        (pointerType.PointeeType.CanonicalType == referenceTypeToMatch.PointeeType.CanonicalType))
                    {
                        continue;
                    }

                    allMatch = false;
                    break;
                }

                if (!allMatch)
                {
                    return false;
                }

                if (cxxMethodDecl.IsVirtual)
                {
                    if (cxxMethodDeclToMatch.IsVirtual)
                    {
                        if (rootCxxRecordDecl != cxxRecordDecl)
                        {
                            // The found declaration and declaration to match are both virtual
                            // We want to treat the one from the base declaration as non-conflicting
                            // So return true to report the declaration to match as the conflict
                            return true;
                        }
                        else if (cxxMethodDeclToMatch.IsThisDeclarationADefinition != cxxMethodDecl.IsThisDeclarationADefinition)
                        {
                            return false;
                        }
                        else
                        {
                            AddDiagnostic(DiagnosticLevel.Error, "Found conflicting method definitions for two virtual methods.", cxxMethodDeclToMatch);
                        }
                    }
                    else
                    {
                        // The found declaration is virtual while the declaration to match is not
                        // We want to treat the virtual declaration as non-conflicting
                        // So return true to report the declaration to match as the conflict
                        return true;
                    }
                }
                else if (cxxMethodDeclToMatch.IsVirtual)
                {
                    // The declaration to match is virtual while the found declaration is not
                    // We want to treat the virtual declaration as non-conflicting
                    // So treat the declaration as non-conflicting and continue searching
                    return false;
                }
                else
                {
                    // Neither the declaration nor the declaration to match are virtual
                    // We want to pick whichever declaration appears first
                    // So return true or false based on if we already encountered the declaration to match
                    return !foundCxxMethodDeclToMatch;
                }

                return false;
            }
        }

        bool IsEmptyRecord(RecordDecl recordDecl)
        {
            if (recordDecl.Fields.Count != 0)
            {
                if (!GetCursorName(recordDecl).EndsWith("__", StringComparison.Ordinal) || (recordDecl.Fields.Count != 1))
                {
                    return false;
                }

                var field = recordDecl.Fields[0];

                if (!GetCursorName(field).Equals("unused", StringComparison.Ordinal) || !IsType<BuiltinType>(field, out var builtinType) || (builtinType.Kind != CXType_Int))
                {
                    return false;
                }
            }

            foreach (var decl in recordDecl.Decls)
            {
                if ((decl is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion && !IsEmptyRecord(nestedRecordDecl))
                {
                    return false;
                }

                if ((decl is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsVirtual)
                {
                    return false;
                }
            }

            if (recordDecl is CXXRecordDecl cxxRecordDecl)
            {
                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                    if (!IsEmptyRecord(baseCxxRecordDecl))
                    {
                        return false;
                    }
                }
            }

            return !TryGetUuid(recordDecl, out _);
        }

        bool IsExcludedByAttributes(Cursor cursor)
        {
            if (cursor is NamedDecl namedDecl)
            {
                foreach (var attr in GetAttributesFor(namedDecl))
                {
                    switch (attr.Kind)
                    {
                        case CX_AttrKind_Builtin:
                            return true;
                    }
                }
            }

            return false;
        }
    }

    private bool IsBaseExcluded(CXXRecordDecl cxxRecordDecl, CXXRecordDecl baseCxxRecordDecl, CXXBaseSpecifier cxxBaseSpecifier, out string baseFieldName)
    {
        baseFieldName = GetAnonymousName(cxxBaseSpecifier, "Base");
        baseFieldName = GetRemappedName(baseFieldName, cxxBaseSpecifier, tryRemapOperatorName: true, out _, skipUsing: true);

        var qualifiedName = $"{GetCursorQualifiedName(cxxRecordDecl)}::{baseFieldName}";
        return _config.ExcludedNames.Contains(qualifiedName);
    }

    private bool IsFixedSize(Cursor cursor, Type type)
    {
        // We don't want to handle these using IsType because we need to specially
        // handle cases like TypedefType at each level of the type hierarchy

        if (type is ArrayType)
        {
            return false;
        }
        else if (type is AttributedType attributedType)
        {
            return IsFixedSize(cursor, attributedType.ModifiedType);
        }
        else if (type is BuiltinType)
        {
            return true;
        }
        else if (type is DecltypeType decltypeType)
        {
            return IsFixedSize(cursor, decltypeType.UnderlyingType);
        }
        else if (type is ElaboratedType elaboratedType)
        {
            return IsFixedSize(cursor, elaboratedType.NamedType);
        }
        else if (type is EnumType enumType)
        {
            return IsFixedSize(cursor, enumType.Decl.IntegerType);
        }
        else if (type is FunctionType)
        {
            return false;
        }
        else if (type is PointerType)
        {
            return false;
        }
        else if (type is RecordType recordType)
        {
            var recordDecl = recordType.Decl;

            return recordDecl.Fields.All((fieldDecl) => IsFixedSize(fieldDecl, fieldDecl.Type))
                && (recordDecl is not CXXRecordDecl cxxRecordDecl || cxxRecordDecl.Methods.All((cxxMethodDecl) => !cxxMethodDecl.IsVirtual));
        }
        else if (type is ReferenceType)
        {
            return false;
        }
        else if (type is TypedefType typedefType)
        {
            var name = GetTypeName(cursor, context: null, type: type, ignoreTransparentStructsWhereRequired: false, isTemplate: false, nativeTypeName: out _);
            var remappedName = GetRemappedTypeName(cursor, context: null, type, out _, skipUsing: true, ignoreTransparentStructsWhereRequired: false);

            return !remappedName.Equals("IntPtr", StringComparison.Ordinal)
                && !remappedName.Equals("nint", StringComparison.Ordinal)
                && !remappedName.Equals("nuint", StringComparison.Ordinal)
                && !remappedName.Equals("UIntPtr", StringComparison.Ordinal)
                && IsFixedSize(cursor, typedefType.Decl.UnderlyingType);
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.TypeClass}'. Assuming unfixed size.", cursor);
            return false;
        }
    }

    private static bool IsNativeTypeNameEquivalent(string nativeTypeName, string typeName)
    {
        return nativeTypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase)
            || nativeTypeName.Replace(" ", "", StringComparison.Ordinal).Equals(typeName, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsPrevContextDecl<T>([MaybeNullWhen(false)] out T cursor, out object? userData, bool includeLast = false)
        where T : Decl
    {
        var previousContext = _context.Last;
        Debug.Assert(previousContext != null);

        if (!includeLast)
        {
            previousContext = previousContext.Previous;
            Debug.Assert(previousContext != null);
        }

        while (previousContext.Value.Cursor is not Decl)
        {
            previousContext = previousContext.Previous;
            Debug.Assert(previousContext != null);
        }

        var value = previousContext.Value;

        if (value.Cursor is T t)
        {
            cursor = t;
            userData = value.UserData;
            return true;
        }
        else
        {
            cursor = null;
            userData = null;
            return false;
        }
    }

    private bool IsPrevContextStmt<T>([MaybeNullWhen(false)] out T cursor, out object? userData, bool preserveParen = false, bool preserveImplicitCast = false)
        where T : Stmt
    {
        var previousContext = _context.Last;
        Debug.Assert(previousContext != null);

        do
        {
            previousContext = previousContext.Previous;
            Debug.Assert(previousContext is not null);
        }
        while ((!preserveParen && (previousContext.Value.Cursor is ParenExpr)) || (!preserveImplicitCast && (previousContext.Value.Cursor is ImplicitCastExpr)));

        var value = previousContext.Value;

        if (value.Cursor is T t)
        {
            cursor = t;
            userData = value.UserData;
            return true;
        }
        else
        {
            cursor = null;
            userData = null;
            return false;
        }
    }

    private bool IsReadonly(CXXMethodDecl? cxxMethodDecl)
    {
        if (cxxMethodDecl is not null)
        {
            return cxxMethodDecl.IsConst || HasRemapping(cxxMethodDecl, _config._withReadonlys, matchStar: true);
        }
        return false;
    }

    private static bool IsStmtAsWritten<T>(Cursor cursor, [MaybeNullWhen(false)] out T value, bool removeParens = false)
        where T : Stmt
    {
        if (cursor is Expr expr)
        {
            cursor = GetExprAsWritten(expr, removeParens);
        }

        if (cursor is T t)
        {
            value = t;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

    private static bool IsStmtAsWritten(Stmt stmt, Stmt expectedStmt, bool removeParens = false)
    {
        if (stmt == expectedStmt)
        {
            return true;
        }

        if (stmt is not Expr expr)
        {
            return false;
        }

        expr = GetExprAsWritten(expr, removeParens);
        return expr == expectedStmt;
    }

    private static bool IsType<T>(Expr expr)
        where T : Type => IsType<T>(expr, out _);

    private static bool IsType<T>(Expr expr, [MaybeNullWhen(false)] out T value)
        where T : Type => IsType(expr, expr.Type, out value);

    private static bool IsType<T>(ValueDecl valueDecl)
        where T : Type => IsType<T>(valueDecl, out _);

    private static bool IsType<T>(ValueDecl typeDecl, [MaybeNullWhen(false)] out T value)
        where T : Type => IsType(typeDecl, typeDecl.Type, out value);

    private static bool IsType<T>(Cursor? cursor, Type type)
        where T : Type => IsType<T>(cursor, type, out _);

    private static bool IsType<T>(Cursor? cursor, Type type, [MaybeNullWhen(false)] out T value)
        where T : Type
    {
        if (type is T t)
        {
            value = t;
            return true;
        }
        else if (type is AttributedType attributedType)
        {
            return IsType(cursor, attributedType.ModifiedType, out value);
        }
        else if (type is DecltypeType decltypeType)
        {
            return IsType(cursor, decltypeType.UnderlyingType, out value);
        }
        else if (type is DeducedType deducedType)
        {
            return IsType(cursor, deducedType.GetDeducedType, out value);
        }
        else if (type is DependentNameType dependentNameType)
        {
            if (dependentNameType.IsSugared)
            {
                return IsType(cursor, dependentNameType.Desugar, out value);
            }
        }
        else if (type is ElaboratedType elaboratedType)
        {
            return IsType(cursor, elaboratedType.NamedType, out value);
        }
        else if (type is InjectedClassNameType injectedClassNameType)
        {
            return IsType(cursor, injectedClassNameType.InjectedTST, out value);
        }
        else if (type is PackExpansionType packExpansionType)
        {
            return IsType(cursor, packExpansionType.Pattern, out value);
        }
        else if (type is SubstTemplateTypeParmType substTemplateTypeParmType)
        {
            return IsType(cursor, substTemplateTypeParmType.ReplacementType, out value);
        }
        else if (type is TemplateSpecializationType templateSpecializationType)
        {
            if (templateSpecializationType.IsTypeAlias)
            {
                return IsType(cursor, templateSpecializationType.AliasedType, out value);
            }
            else if (templateSpecializationType.IsSugared)
            {
                return IsType(cursor, templateSpecializationType.Desugar, out value);
            }
            else if (templateSpecializationType.TemplateName.AsTemplateDecl is TemplateDecl templateDecl)
            {
                // We exclude InjectedClassNameType here to avoid infinite recursion.
                if ((templateDecl.TemplatedDecl is TypeDecl typeDecl) && (typeDecl.TypeForDecl is not InjectedClassNameType ))
                {
                    return IsType(cursor, typeDecl.TypeForDecl, out value);
                }
            }
        }
        else if (type is TemplateTypeParmType templateTypeParmType)
        {
            if (templateTypeParmType.IsSugared)
            {
                return IsType(cursor, templateTypeParmType.Decl.TypeForDecl, out value);
            }
        }
        else if (type is TypedefType typedefType)
        {
            return IsType(cursor, typedefType.Decl.UnderlyingType, out value);
        }
        else if (type is UsingType usingType)
        {
            if (usingType.IsSugared)
            {
                return IsType(cursor, usingType.Desugar, out value);
            }
        }

        value = default;
        return false;
    }

    private static bool IsTypeConstantOrIncompleteArray(Expr expr)
         => IsTypeConstantOrIncompleteArray(expr, out _);

    private static bool IsTypeConstantOrIncompleteArray(Expr expr, [MaybeNullWhen(false)] out ArrayType arrayType)
         => IsTypeConstantOrIncompleteArray(expr, expr.Type, out arrayType);

    private bool IsTypeConstantOrIncompleteArray(ValueDecl valueDecl)
         => IsTypeConstantOrIncompleteArray(valueDecl, out _);

    private static bool IsTypeConstantOrIncompleteArray(ValueDecl valueDecl, [MaybeNullWhen(false)] out ArrayType arrayType)
         => IsTypeConstantOrIncompleteArray(valueDecl, valueDecl.Type, out arrayType);

    private static bool IsTypeConstantOrIncompleteArray(Cursor? cursor, Type type)
         => IsTypeConstantOrIncompleteArray(cursor, type, out _);

    private static bool IsTypeConstantOrIncompleteArray(Cursor? cursor, Type type, [MaybeNullWhen(false)] out ArrayType arrayType)
         => IsType(cursor, type, out arrayType)
         && (arrayType is ConstantArrayType or IncompleteArrayType);

    private static bool IsTypePointerOrReference(Expr expr)
         => IsTypePointerOrReference(expr, expr.Type);

    private static bool IsTypePointerOrReference(ValueDecl valueDecl)
        => IsTypePointerOrReference(valueDecl, valueDecl.Type);

    private static bool IsTypePointerOrReference(Cursor? cursor, Type type)
        => IsType<PointerType>(cursor, type)
        || IsType<ReferenceType>(cursor, type);

    private static bool IsTypeVoid(Cursor? cursor, Type type)
         => IsType<BuiltinType>(cursor, type, out var builtinType)
         && (builtinType.Kind == CXType_Void);

    internal bool IsSupportedFixedSizedBufferType(string typeName)
    {
        switch (typeName)
        {
            case "bool":
            case "byte":
            case "char":
            case "double":
            case "float":
            case "int":
            case "long":
            case "sbyte":
            case "short":
            case "ushort":
            case "uint":
            case "ulong":
            {
                // We want to prefer InlineArray in modern code, as it is safer and supports more features
                return Config.GenerateCompatibleCode;
            }

            default:
            {
                return false;
            }
        }
    }

    private static bool IsTransparentStructBoolean(PInvokeGeneratorTransparentStructKind kind)
        => kind is PInvokeGeneratorTransparentStructKind.Boolean;

    private static bool IsTransparentStructHandle(PInvokeGeneratorTransparentStructKind kind)
         => kind is PInvokeGeneratorTransparentStructKind.Handle
                  or PInvokeGeneratorTransparentStructKind.HandleWin32;

    private static bool IsTransparentStructHexBased(PInvokeGeneratorTransparentStructKind kind)
         => IsTransparentStructHandle(kind)
         || (kind == PInvokeGeneratorTransparentStructKind.TypedefHex);

    private bool IsUnchecked(string targetTypeName, Stmt stmt)
    {
        if (IsPrevContextDecl<VarDecl>(out var parentVarDecl, out _))
        {
            var cursorName = GetCursorName(parentVarDecl);

            if (cursorName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal) && _config.WithTransparentStructs.TryGetValue(targetTypeName, out var transparentStruct))
            {
                targetTypeName = transparentStruct.Name;
            }
        }

        switch (stmt.StmtClass)
        {
            // case CX_StmtClass_BinaryConditionalOperator:

            case CX_StmtClass_ConditionalOperator:
            {
                var conditionalOperator = (ConditionalOperator)stmt;
                return IsUnchecked(targetTypeName, conditionalOperator.LHS)
                    || IsUnchecked(targetTypeName, conditionalOperator.RHS)
                    || IsUnchecked(targetTypeName, conditionalOperator.Handle.Evaluate);
            }

            // case CX_StmtClass_AddrLabelExpr:
            // case CX_StmtClass_ArrayInitIndexExpr:
            // case CX_StmtClass_ArrayInitLoopExpr:

            case CX_StmtClass_ArraySubscriptExpr:
            {
                var arraySubscriptExpr = (ArraySubscriptExpr)stmt;
                return IsUnchecked(targetTypeName, arraySubscriptExpr.LHS)
                    || IsUnchecked(targetTypeName, arraySubscriptExpr.RHS);
            }

            // case CX_StmtClass_ArrayTypeTraitExpr:
            // case CX_StmtClass_AsTypeExpr:
            // case CX_StmtClass_AtomicExpr:

            case CX_StmtClass_BinaryOperator:
            {
                var binaryOperator = (BinaryOperator)stmt;
                return IsUnchecked(targetTypeName, binaryOperator.LHS)
                    || IsUnchecked(targetTypeName, binaryOperator.RHS)
                    || IsUnchecked(targetTypeName, binaryOperator.Handle.Evaluate)
                    || IsOverflow(binaryOperator);
            }

            // case CX_StmtClass_CompoundAssignOperator:
            // case CX_StmtClass_BlockExpr:
            // case CX_StmtClass_CXXBindTemporaryExpr:

            case CX_StmtClass_CXXBoolLiteralExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXConstructExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXTemporaryObjectExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXDefaultArgExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXDefaultInitExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXDeleteExpr:

            case CX_StmtClass_CXXDependentScopeMemberExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXFoldExpr:
            // case CX_StmtClass_CXXInheritedCtorInitExpr:

            case CX_StmtClass_CXXNewExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXNoexceptExpr:

            case CX_StmtClass_CXXNullPtrLiteralExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXPseudoDestructorExpr:
            // case CX_StmtClass_CXXRewrittenBinaryOperator:
            // case CX_StmtClass_CXXScalarValueInitExpr:
            // case CX_StmtClass_CXXStdInitializerListExpr:

            case CX_StmtClass_CXXThisExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXThrowExpr:
            // case CX_StmtClass_CXXTypeidExpr:
            // case CX_StmtClass_CXXUnresolvedConstructExpr:

            case CX_StmtClass_CXXUuidofExpr:
            {
                return false;
            }

            case CX_StmtClass_CallExpr:
            {
                return false;
            }

            // case CX_StmtClass_CUDAKernelCallExpr:

            case CX_StmtClass_CXXMemberCallExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXOperatorCallExpr:
            {
                return false;
            }

            // case CX_StmtClass_UserDefinedLiteral:
            // case CX_StmtClass_BuiltinBitCastExpr:

            case CX_StmtClass_CStyleCastExpr:
            case CX_StmtClass_CXXStaticCastExpr:
            case CX_StmtClass_CXXFunctionalCastExpr:
            {
                var explicitCastExpr = (ExplicitCastExpr)stmt;
                var explicitCastExprTypeName = GetRemappedTypeName(explicitCastExpr, context: null, explicitCastExpr.Type, out _);

                return IsUnchecked(targetTypeName, explicitCastExpr.SubExprAsWritten)
                    || IsUnchecked(targetTypeName, explicitCastExpr.Handle.Evaluate)
                    || (IsUnsigned(targetTypeName) != IsUnsigned(explicitCastExprTypeName));
            }

            case CX_StmtClass_CXXConstCastExpr:
            case CX_StmtClass_CXXDynamicCastExpr:
            case CX_StmtClass_CXXReinterpretCastExpr:
            {
                var namedCastExpr = (CXXNamedCastExpr)stmt;

                return IsUnchecked(targetTypeName, namedCastExpr.SubExprAsWritten)
                    || IsUnchecked(targetTypeName, namedCastExpr.Handle.Evaluate);
            }

            // case CX_StmtClass_ObjCBridgedCastExpr:

            case CX_StmtClass_ImplicitCastExpr:
            {
                var implicitCastExpr = (ImplicitCastExpr)stmt;

                return IsUnchecked(targetTypeName, implicitCastExpr.SubExprAsWritten)
                    || IsUnchecked(targetTypeName, implicitCastExpr.Handle.Evaluate);
            }

            case CX_StmtClass_CharacterLiteral:
            {
                return false;
            }

            // case CX_StmtClass_ChooseExpr:
            // case CX_StmtClass_CompoundLiteralExpr:
            // case CX_StmtClass_ConceptSpecializationExpr:
            // case CX_StmtClass_ConvertVectorExpr:
            // case CX_StmtClass_CoawaitExpr:
            // case CX_StmtClass_CoyieldExpr:

            case CX_StmtClass_DeclRefExpr:
            {
                var declRefExpr = (DeclRefExpr)stmt;
                return (declRefExpr.Decl is VarDecl varDecl) && varDecl.HasInit && IsUnchecked(targetTypeName, varDecl.Init);
            }

            // case CX_StmtClass_DependentCoawaitExpr:
            // case CX_StmtClass_DependentScopeDeclRefExpr:
            // case CX_StmtClass_DesignatedInitExpr:
            // case CX_StmtClass_DesignatedInitUpdateExpr:
            // case CX_StmtClass_ExpressionTraitExpr:
            // case CX_StmtClass_ExtVectorElementExpr:
            // case CX_StmtClass_FixedPointLiteral:

            case CX_StmtClass_FloatingLiteral:
            {
                return false;
            }

            // case CX_StmtClass_ConstantExpr:

            case CX_StmtClass_ExprWithCleanups:
            {
                var exprWithCleanups = (ExprWithCleanups)stmt;
                return IsUnchecked(targetTypeName, exprWithCleanups.SubExpr);
            }

            // case CX_StmtClass_FunctionParmPackExpr:
            // case CX_StmtClass_GNUNullExpr:
            // case CX_StmtClass_GenericSelectionExpr:
            // case CX_StmtClass_ImaginaryLiteral:
            // case CX_StmtClass_ImplicitValueInitExpr:

            case CX_StmtClass_InitListExpr:
            {
                return false;
            }

            case CX_StmtClass_IntegerLiteral:
            {
                var integerLiteral = (IntegerLiteral)stmt;
                var signedValue = integerLiteral.Value;
                return IsUnchecked(targetTypeName, signedValue, integerLiteral.IsNegative, isHex: integerLiteral.ValueString.StartsWith("0x", StringComparison.Ordinal));
            }

            case CX_StmtClass_LambdaExpr:
            {
                return false;
            }

            // case CX_StmtClass_MSPropertyRefExpr:
            // case CX_StmtClass_MSPropertySubscriptExpr:

            case CX_StmtClass_MaterializeTemporaryExpr:
            {
                return false;
            }

            case CX_StmtClass_MemberExpr:
            {
                return false;
            }

            // case CX_StmtClass_NoInitExpr:
            // case CX_StmtClass_ArraySectionExpr:
            // case CX_StmtClass_ObjCArrayLiteral:
            // case CX_StmtClass_ObjCAvailabilityCheckExpr:
            // case CX_StmtClass_ObjCBoolLiteralExpr:
            // case CX_StmtClass_ObjCBoxedExpr:
            // case CX_StmtClass_ObjCDictionaryLiteral:
            // case CX_StmtClass_ObjCEncodeExpr:
            // case CX_StmtClass_ObjCIndirectCopyRestoreExpr:
            // case CX_StmtClass_ObjCIsaExpr:
            // case CX_StmtClass_ObjCIvarRefExpr:
            // case CX_StmtClass_ObjCMessageExpr:
            // case CX_StmtClass_ObjCPropertyRefExpr:
            // case CX_StmtClass_ObjCProtocolExpr:
            // case CX_StmtClass_ObjCSelectorExpr:
            // case CX_StmtClass_ObjCStringLiteral:
            // case CX_StmtClass_ObjCSubscriptRefExpr:

            case CX_StmtClass_OffsetOfExpr:
            {
                return false;
            }

            // case CX_StmtClass_OpaqueValueExpr:
            // case CX_StmtClass_UnresolvedLookupExpr:
            // case CX_StmtClass_UnresolvedMemberExpr:
            // case CX_StmtClass_PackExpansionExpr:

            case CX_StmtClass_ParenExpr:
            {
                var parenExpr = (ParenExpr)stmt;
                return IsUnchecked(targetTypeName, parenExpr.SubExpr)
                    || IsUnchecked(targetTypeName, parenExpr.Handle.Evaluate);
            }

            case CX_StmtClass_ParenListExpr:
            {
                var parenListExpr = (ParenListExpr)stmt;

                foreach (var expr in parenListExpr.Exprs)
                {
                    if (IsUnchecked(targetTypeName, expr) || IsUnchecked(targetTypeName, expr.Handle.Evaluate))
                    {
                        return true;
                    }
                }

                return false;
            }

            // case CX_StmtClass_PredefinedExpr:
            // case CX_StmtClass_PseudoObjectExpr:
            // case CX_StmtClass_RequiresExpr:
            // case CX_StmtClass_ShuffleVectorExpr:
            // case CX_StmtClass_SizeOfPackExpr:
            // case CX_StmtClass_SourceLocExpr:
            // case CX_StmtClass_StmtExpr:

            case CX_StmtClass_StringLiteral:
            {
                return false;
            }

            case CX_StmtClass_SubstNonTypeTemplateParmExpr:
            {
                return false;
            }

            // case CX_StmtClass_SubstNonTypeTemplateParmPackExpr:
            // case CX_StmtClass_TypeTraitExpr:
            // case CX_StmtClass_TypoExpr:

            case CX_StmtClass_UnaryExprOrTypeTraitExpr:
            {
                var unaryExprOrTypeTraitExpr = (UnaryExprOrTypeTraitExpr)stmt;

                var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;

                long alignment32 = -1;
                long alignment64 = -1;

                GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out var size32, out var size64);

                switch (unaryExprOrTypeTraitExpr.Kind)
                {
                    case CX_UETT_SizeOf:
                    {
                        switch (targetTypeName)
                        {
                            case "bool":
                            case "Boolean":
                            case "byte":
                            case "Byte":
                            case "char":
                            case "Char":
                            case "ushort":
                            case "UInt16":
                            case "uint":
                            case "UInt32":
                            case "nuint":
                            case "sbyte":
                            case "SByte":
                            case "short":
                            case "Int16":
                            {
                                return (size32 != size64) || !IsPrevContextDecl<VarDecl>(out _, out _);
                            }

                            case "ulong":
                            case "UInt64":
                            case "int":
                            case "Int32":
                            case "nint":
                            case "long":
                            case "Int64":
                            {
                                return false;
                            }

                            default:
                            {
                                return false;
                            }
                        }
                    }

                    default:
                    {
                        return false;
                    }
                }
            }

            case CX_StmtClass_UnaryOperator:
            {
                var unaryOperator = (UnaryOperator)stmt;

                if (IsUnchecked(targetTypeName, unaryOperator.SubExpr))
                {
                    return true;
                }

                var evaluation = unaryOperator.Handle.Evaluate;

                if (IsUnchecked(targetTypeName, evaluation))
                {
                    return true;
                }

                var sourceTypeName = GetTypeName(stmt, context: null, type: unaryOperator.SubExpr.Type, ignoreTransparentStructsWhereRequired: false, isTemplate: false, nativeTypeName: out _);

                switch (unaryOperator.Opcode)
                {
                    case CXUnaryOperator_Minus:
                    {
                        return IsUnsigned(targetTypeName);
                    }

                    case CXUnaryOperator_Not:
                    {
                        return IsUnsigned(targetTypeName) != IsUnsigned(sourceTypeName);
                    }

                    default:
                    {
                        return false;
                    }
                }
            }

            // case CX_StmtClass_VAArgExpr:

            default:
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported statement class: '{stmt.StmtClassName}'. Generated bindings may not be unchecked.", stmt);
                return false;
            }
        }

        bool IsOverflow(BinaryOperator binaryOperator)
        {
            var lhs = binaryOperator.LHS;
            var rhs = binaryOperator.RHS;

            long lhsValue, rhsValue;

            if (IsStmtAsWritten<IntegerLiteral>(lhs, out var lhsIntegerLiteral, removeParens: true))
            {
                lhsValue = lhsIntegerLiteral.Value;
            }
            else
            {
                var lhsEvaluation = lhs.Handle.Evaluate;

                if (lhsEvaluation.Kind == CXEval_Int)
                {
                    lhsValue = lhsEvaluation.AsInt;
                }
                else
                {
                    return false;
                }
            }

            if (IsStmtAsWritten<IntegerLiteral>(rhs, out var rhsIntegerLiteral, removeParens: true))
            {
                rhsValue = rhsIntegerLiteral.Value;
            }
            else
            {
                var rhsEvaluation = rhs.Handle.Evaluate;

                if (rhsEvaluation.Kind == CXEval_Int)
                {
                    rhsValue = rhsEvaluation.AsInt;
                }
                else
                {
                    return false;
                }
            }

            var targetTypeName = GetRemappedTypeName(binaryOperator, context: null, binaryOperator.Type, out _, skipUsing: true);
            var isUnsigned = IsUnsigned(targetTypeName);

            switch (binaryOperator.Opcode)
            {
                case CXBinaryOperator_Add:
                {
                    return isUnsigned
                        ? (ulong)lhsValue + (ulong)rhsValue < (ulong)lhsValue
                        : lhsValue + rhsValue < lhsValue;
                }

                case CXBinaryOperator_Sub:
                {
                    return isUnsigned
                        ? (ulong)lhsValue - (ulong)rhsValue > (ulong)lhsValue
                        : lhsValue - rhsValue > lhsValue;
                }

                default:
                {
                    return false;
                }
            }
        }
    }

    private static bool IsUnchecked(string typeName, CXEvalResult evalResult)
    {
        if (evalResult.Kind != CXEval_Int)
        {
            return false;
        }

        var signedValue = evalResult.AsLongLong;
        return IsUnchecked(typeName, signedValue, signedValue < 0, isHex: false);
    }

    private static bool IsUnchecked(string typeName, long signedValue, bool isNegative, bool isHex)
    {
        switch (typeName)
        {
            case "byte":
            case "Byte":
            {
                var unsignedValue = unchecked((ulong)signedValue);
                return unsignedValue is < byte.MinValue or > byte.MaxValue;
            }

            case "char":
            case "Char":
            {
                var unsignedValue = unchecked((ulong)signedValue);
                return unsignedValue is < char.MinValue or > char.MaxValue;
            }

            case "ushort":
            case "UInt16":
            {
                var unsignedValue = unchecked((ulong)signedValue);
                return unsignedValue is < ushort.MinValue or > ushort.MaxValue;
            }

            case "uint":
            case "UInt32":
            case "nuint":
            case "UIntPtr":
            {
                return false;
            }

            case "ulong":
            case "UInt64":
            {
                return false;
            }

            case "sbyte":
            case "SByte":
            {
                return (signedValue < sbyte.MinValue) || (sbyte.MaxValue < signedValue) || (isNegative && isHex);
            }

            case "short":
            case "Int16":
            {
                return (signedValue < short.MinValue) || (short.MaxValue < signedValue) || (isNegative && isHex);
            }

            case "int":
            case "Int32":
            case "nint":
            case "IntPtr":
            {
                return (signedValue < int.MinValue) || (int.MaxValue < signedValue) || (isNegative && isHex);
            }

            case "long":
            case "Int64":
            {
                return (signedValue < long.MinValue) || (long.MaxValue < signedValue) || (isNegative && isHex);
            }

            default:
            {
                return false;
            }
        }
    }

    private bool IsUnsafe(FieldDecl fieldDecl)
    {
        var type = fieldDecl.Type;

        if (IsType<ArrayType>(fieldDecl, out _) && IsTypeConstantOrIncompleteArray(fieldDecl, type))
        {
            var remappedName = GetRemappedTypeName(fieldDecl, context: null, type, out _, skipUsing: true, ignoreTransparentStructsWhereRequired: false);
            return IsSupportedFixedSizedBufferType(remappedName);
        }

        return IsUnsafe(fieldDecl, type);
    }

    private bool IsUnsafe(FunctionDecl functionDecl)
    {
        var name = GetRemappedCursorName(functionDecl);

        if (_config.WithManualImports.Contains(name))
        {
            return true;
        }

        if (IsUnsafe(functionDecl, functionDecl.ReturnType))
        {
            return true;
        }

        foreach (var parmVarDecl in functionDecl.Parameters)
        {
            if (IsUnsafe(parmVarDecl))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsUnsafe(ParmVarDecl parmVarDecl)
    {
        var type = parmVarDecl.Type;
        return IsUnsafe(parmVarDecl, type);
    }

    private bool IsUnsafe(RecordDecl recordDecl)
    {
        foreach (var decl in recordDecl.Decls)
        {
            if ((decl is FieldDecl fieldDecl) && IsUnsafe(fieldDecl))
            {
                return true;
            }
            else if ((decl is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion && (IsUnsafe(nestedRecordDecl) || Config.GenerateCompatibleCode))
            {
                return true;
            }
        }
        return (recordDecl is CXXRecordDecl cxxRecordDecl) && (HasVtbl(cxxRecordDecl, out var hasBaseVtbl) || hasBaseVtbl || HasUnsafeMethod(cxxRecordDecl));
    }

    private bool IsUnsafe(TypedefDecl typedefDecl, FunctionProtoType functionProtoType)
    {
        var returnType = functionProtoType.ReturnType;

        if (IsUnsafe(typedefDecl, returnType))
        {
            return true;
        }

        foreach (var paramType in functionProtoType.ParamTypes)
        {
            if (IsUnsafe(typedefDecl, paramType))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsUnsafe(NamedDecl namedDecl, Type type)
    {
        var remappedName = GetRemappedTypeName(namedDecl, context: null, type, out _, skipUsing: true, ignoreTransparentStructsWhereRequired: false);
        return remappedName.Contains('*', StringComparison.Ordinal);
    }

    private static bool IsUnsigned(string typeName)
    {
        switch (typeName)
        {
            case "byte":
            case "Byte":
            case "char":
            case "Char":
            case "nuint":
            case "UInt16":
            case "uint":
            case "UInt32":
            case "ulong":
            case "UInt64":
            case "UIntPtr":
            case "ushort":
            case var _ when typeName.EndsWith('*'):
            {
                return true;
            }

            case "Int16":
            case "int":
            case "Int32":
            case "long":
            case "Int64":
            case "nint":
            case "sbyte":
            case "SByte":
            case "short":
            {
                return false;
            }

            default:
            {
                return false;
            }
        }
    }
}
