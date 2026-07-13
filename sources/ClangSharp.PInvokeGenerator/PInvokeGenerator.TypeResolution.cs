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
    private string GetTargetTypeName(Cursor cursor, out string nativeTypeName)
    {
        var targetTypeName = "";
        nativeTypeName = "";

        if (cursor is Decl decl)
        {
            if (decl is EnumConstantDecl enumConstantDecl)
            {
                targetTypeName = enumConstantDecl.DeclContext is EnumDecl enumDecl
                               ? GetRemappedTypeName(enumDecl, context: null, enumDecl.IntegerType, out nativeTypeName)
                               : GetRemappedTypeName(enumConstantDecl, context: null, enumConstantDecl.Type, out nativeTypeName);
            }
            else if (decl is TypeDecl previousTypeDecl)
            {
                targetTypeName = GetRemappedTypeName(previousTypeDecl, context: null, previousTypeDecl.TypeForDecl, out nativeTypeName);
            }
            else if (decl is VarDecl varDecl)
            {
                if (varDecl is ParmVarDecl parmVarDecl)
                {
                    targetTypeName = GetRemappedTypeName(parmVarDecl, context: null, parmVarDecl.Type, out nativeTypeName);

                    if (!_config.GenerateDisableRuntimeMarshalling && (parmVarDecl.ParentFunctionOrMethod is FunctionDecl functionDecl) && (((functionDecl is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsVirtual) || (functionDecl.Body is null)) && targetTypeName.Equals("bool", StringComparison.Ordinal))
                    {
                        // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                        targetTypeName = "byte";
                        nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? "bool" : nativeTypeName;
                    }
                }
                else
                {
                    var type = varDecl.Type;
                    var cursorName = GetCursorName(varDecl).AsSpan();

                    if (cursorName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
                    {
                        cursorName = cursorName["ClangSharpMacro_".Length..];

                        if (_config._withTypes.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(cursorName, out targetTypeName))
                        {
                            return targetTypeName;
                        }

                        type = varDecl.Init.Type;
                    }

                    targetTypeName = GetRemappedTypeName(varDecl, context: null, type, out nativeTypeName);
                }
            }

        }
        else if ((cursor is Expr expr) && (expr is not MemberExpr))
        {
            targetTypeName = GetRemappedTypeName(expr, context: null, expr.Type, out nativeTypeName);
        }

        return targetTypeName;
    }

    private string GetTypeName(Cursor? cursor, Cursor? context, Type type, bool ignoreTransparentStructsWhereRequired, bool isTemplate, out string nativeTypeName)
    {
        if (_typeNames.TryGetValue((cursor, context, type), out var result))
        {
            nativeTypeName = result.nativeTypeName;
            return result.typeName;
        }
        else if (IsType<TagType>(cursor, type, out var tagType) && tagType.Decl.Handle.IsAnonymous)
        {
            // In order to avoid minor path differences, casing, and other deltas across different
            // invocations of the tool, we want to use the "built" anonymous name so we get a more
            // minimal but still accurate set of information embedded in the output.

            result.typeName = GetAnonymousName(tagType.Decl, tagType.KindSpelling);
            result.nativeTypeName = result.typeName;

            _typeNames[(cursor, context, type)] = result;

            nativeTypeName = result.nativeTypeName;
            return result.typeName;
        }
        else
        {
            return GetTypeName(cursor, context, type, type, ignoreTransparentStructsWhereRequired, isTemplate, out nativeTypeName);
        }
    }

    private string GetTypeName(Cursor? cursor, Cursor? context, Type rootType, Type type, bool ignoreTransparentStructsWhereRequired, bool isTemplate, out string nativeTypeName)
    {
        if (!_typeNames.TryGetValue((cursor, context, type), out var result))
        {
            result.typeName = type.AsString.NormalizePath()
                                           .Replace("unnamed enum at", "anonymous enum at", StringComparison.Ordinal)
                                           .Replace("unnamed struct at", "anonymous struct at", StringComparison.Ordinal)
                                           .Replace("unnamed union at", "anonymous union at", StringComparison.Ordinal);

            result.nativeTypeName = result.typeName;

            // We don't want to handle these using IsType because we need to specially
            // handle cases like TypedefType at each level of the type hierarchy

            if (type is ArrayType arrayType)
            {
                result.typeName = GetRemappedTypeName(cursor, context, arrayType.ElementType, out _, skipUsing: true, ignoreTransparentStructsWhereRequired);

                if (cursor is FunctionDecl or ParmVarDecl)
                {
                    result.typeName += '*';
                }
            }
            else if (type is AttributedType attributedType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, attributedType.ModifiedType, ignoreTransparentStructsWhereRequired, isTemplate, out _);
            }
            else if (type is BuiltinType)
            {
                switch (type.Kind)
                {
                    case CXType_Void:
                    {
                        result.typeName = (cursor is null) ? "Void" : "void";
                        break;
                    }

                    case CXType_Bool:
                    {
                        result.typeName = (cursor is null) ? "Boolean" : "bool";
                        break;
                    }

                    case CXType_Char_U:
                    case CXType_UChar:
                    {
                        result.typeName = (cursor is null) ? "Byte" : "byte";
                        break;
                    }

                    case CXType_Char16:
                    {
                        if (_config.GenerateDisableRuntimeMarshalling)
                        {
                            result.typeName = (cursor is null) ? "Char" : "char";
                            break;
                        }
                        goto case CXType_UShort;
                    }

                    case CXType_UShort:
                    {
                        result.typeName = (cursor is null) ? "UInt16" : "ushort";
                        break;
                    }

                    case CXType_UInt:
                    {
                        result.typeName = (cursor is null) ? "UInt32" : "uint";
                        break;
                    }

                    case CXType_ULong:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            result.typeName = _config.ExcludeNIntCodegen ? "UIntPtr" : "nuint";
                        }
                        else
                        {
                            goto case CXType_UInt;
                        }
                        break;
                    }

                    case CXType_ULongLong:
                    {
                        result.typeName = (cursor is null) ? "UInt64" : "ulong";
                        break;
                    }

                    case CXType_Char_S:
                    case CXType_SChar:
                    {
                        result.typeName = (cursor is null) ? "SByte" : "sbyte";
                        break;
                    }

                    case CXType_WChar:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto case CXType_UInt;
                        }
                        else
                        {
                            goto case CXType_Char16;
                        }
                    }

                    case CXType_Short:
                    {
                        result.typeName = (cursor is null) ? "Int16" : "short";
                        break;
                    }

                    case CXType_Int:
                    {
                        result.typeName = (cursor is null) ? "Int32" : "int";
                        break;
                    }

                    case CXType_Long:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            result.typeName = _config.ExcludeNIntCodegen ? "IntPtr" : "nint";
                        }
                        else
                        {
                            goto case CXType_Int;
                        }
                        break;
                    }

                    case CXType_LongLong:
                    {
                        result.typeName = (cursor is null) ? "Int64" : "long";
                        break;
                    }

                    case CXType_Float:
                    {
                        result.typeName = (cursor is null) ? "Single" : "float";
                        break;
                    }

                    case CXType_Double:
                    {
                        result.typeName = (cursor is null) ? "Double" : "double";
                        break;
                    }

                    case CXType_NullPtr:
                    {
                        result.typeName = "void*";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported builtin type: '{type.KindSpelling}'. Falling back '{result.typeName}'.", cursor);
                        break;
                    }
                }
            }
            else if (type is DecltypeType decltypeType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, decltypeType.UnderlyingType, ignoreTransparentStructsWhereRequired, isTemplate, out _);
            }
            else if (type is DeducedType deducedType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, deducedType.GetDeducedType, ignoreTransparentStructsWhereRequired, isTemplate, out _);
            }
            else if (type is DependentNameType dependentNameType)
            {
                if (dependentNameType.IsSugared)
                {
                    result.typeName = GetTypeName(cursor, context, rootType, dependentNameType.Desugar, ignoreTransparentStructsWhereRequired, isTemplate, out _);
                }
                else
                {
                    // The default name should be correct
                }
            }
            else if (type is ElaboratedType elaboratedType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, elaboratedType.NamedType, ignoreTransparentStructsWhereRequired, isTemplate, out var nativeNamedTypeName);

                if (!string.IsNullOrWhiteSpace(nativeNamedTypeName) &&
                    !result.nativeTypeName.StartsWith("const ", StringComparison.Ordinal) &&
                    !result.nativeTypeName.StartsWith("enum ", StringComparison.Ordinal) &&
                    !result.nativeTypeName.StartsWith("struct ", StringComparison.Ordinal) &&
                    !result.nativeTypeName.StartsWith("union ", StringComparison.Ordinal))
                {
                    result.nativeTypeName = nativeNamedTypeName;
                }
            }
            else if (type is FunctionType functionType)
            {
                result.typeName = GetTypeNameForPointeeType(cursor, context, rootType, functionType, ignoreTransparentStructsWhereRequired, isTemplate, out _, out _);
            }
            else if (type is InjectedClassNameType injectedClassNameType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, injectedClassNameType.InjectedTST, ignoreTransparentStructsWhereRequired, isTemplate, out _);
            }
            else if (type is PackExpansionType packExpansionType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, packExpansionType.Pattern, ignoreTransparentStructsWhereRequired, isTemplate, out _);
            }
            else if (type is PointerType pointerType)
            {
                result.typeName = GetTypeNameForPointeeType(cursor, context, rootType, pointerType.PointeeType, ignoreTransparentStructsWhereRequired, isTemplate, out var nativePointeeTypeName, out var isAdjusted);

                if (isAdjusted)
                {
                    result.nativeTypeName = $"{nativePointeeTypeName} *";
                }
            }
            else if (type is ReferenceType referenceType)
            {
                result.typeName = GetTypeNameForPointeeType(cursor, context, rootType, referenceType.PointeeType, ignoreTransparentStructsWhereRequired, isTemplate, out var nativePointeeTypeName, out var isAdjusted);

                if (isAdjusted)
                {
                    result.nativeTypeName = $"{nativePointeeTypeName} &";
                }
            }
            else if (type is SubstTemplateTypeParmType substTemplateTypeParmType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, substTemplateTypeParmType.ReplacementType, ignoreTransparentStructsWhereRequired, isTemplate, out _);
            }
            else if (type is TagType tagType)
            {
                if (tagType.Decl.Handle.IsAnonymous)
                {
                    // In order to avoid minor path differences, casing, and other deltas across different
                    // invocations of the tool, we want to use the "built" anonymous name so we get a more
                    // minimal but still accurate set of information embedded in the output.

                    result.typeName = GetAnonymousName(tagType.Decl, tagType.KindSpelling);
                    result.nativeTypeName = result.typeName;
                }
                else if (tagType.Handle.IsConstQualified)
                {
                    result.typeName = GetTypeName(cursor, context, rootType, tagType.Decl.TypeForDecl, ignoreTransparentStructsWhereRequired, isTemplate, out _);
                }
                else
                {
                    // The default name should be correct for C++, but C may have a prefix we need to strip

                    if (result.typeName.StartsWith("enum ", StringComparison.Ordinal))
                    {
                        result.typeName = result.typeName[5..];
                    }
                    else if (result.typeName.StartsWith("struct ", StringComparison.Ordinal))
                    {
                        result.typeName = result.typeName[7..];
                    }
                    else if (result.typeName.StartsWith("union ", StringComparison.Ordinal))
                    {
                        result.typeName = result.typeName[6..];
                    }
                }

                if (result.typeName.Contains("::", StringComparison.Ordinal))
                {
                    result.typeName = result.typeName.Split(s_doubleColonSeparator, StringSplitOptions.RemoveEmptyEntries).Last();
                    result.typeName = GetRemappedName(result.typeName, cursor, tryRemapOperatorName: false, out _, skipUsing: true);
                    result.typeName = ApplyTagTypeNameOverrides(tagType, result.typeName);

                    // A nested type needs to be qualified by its containing type(s) so it resolves
                    // when referenced from another scope (e.g. `A::Inner` -> `A.Inner`). Namespaces
                    // are flattened away, so only walk the enclosing record decls.

                    var qualificationBuilder = new StringBuilder();

                    for (var declContext = tagType.Decl.DeclContext; declContext is RecordDecl parentRecordDecl; declContext = parentRecordDecl.DeclContext)
                    {
                        var parentRecordDeclName = GetRemappedCursorName(parentRecordDecl, out _, skipUsing: true);
                        _ = qualificationBuilder.Insert(0, '.').Insert(0, EscapeName(parentRecordDeclName));
                    }

                    result.typeName = qualificationBuilder.Append(result.typeName).ToString();
                }
                else
                {
                    result.typeName = ApplyTagTypeNameOverrides(tagType, result.typeName);
                }
            }
            else if (type is TemplateSpecializationType templateSpecializationType)
            {
                var nameBuilder = new StringBuilder();

                var templateTypeDecl = IsType<RecordType>(cursor, templateSpecializationType, out var recordType)
                                     ? recordType.Decl
                                     : (NamedDecl)templateSpecializationType.TemplateName.AsTemplateDecl;

                var templateTypeDeclName = GetRemappedCursorName(templateTypeDecl, out _, skipUsing: true);
                var isStdAtomic = false;

                if (templateTypeDeclName.Equals("atomic", StringComparison.Ordinal))
                {
                    isStdAtomic = (templateTypeDecl.Parent is NamespaceDecl namespaceDecl) && namespaceDecl.IsStdNamespace;
                }

                if (!isStdAtomic)
                {
                    _ = nameBuilder.Append(templateTypeDeclName);
                    _ = nameBuilder.Append('<');
                }
                else
                {
                    _ = nameBuilder.Append("volatile ");
                }

                var shouldWritePrecedingComma = false;

                foreach (var arg in templateSpecializationType.Args)
                {
                    if (shouldWritePrecedingComma)
                    {
                        _ = nameBuilder.Append(',');
                        _ = nameBuilder.Append(' ');
                    }

                    var typeName = "";

                    switch (arg.Kind)
                    {
                        case CXTemplateArgumentKind_Type:
                        {
                            typeName = GetRemappedTypeName(cursor, context: null, arg.AsType, out var nativeAsTypeName, skipUsing: true, isTemplate: true);
                            break;
                        }

                        case CXTemplateArgumentKind_Expression:
                        {
                            var oldOutputBuilder = _outputBuilder;
                            _outputBuilder = new CSharpOutputBuilder("ClangSharp_TemplateSpecializationType_AsExpr", this);

                            Visit(arg.AsExpr);
                            typeName = _outputBuilder.ToString() ?? "";

                            _outputBuilder = oldOutputBuilder;
                            break;
                        }

                        default:
                        {
                            typeName = result.typeName;
                            AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported template argument kind: '{arg.Kind}'. Falling back '{result.typeName}'.", cursor);
                            break;
                        }
                    }

                    if (!_config.GenerateDisableRuntimeMarshalling && typeName.Equals("bool", StringComparison.Ordinal))
                    {
                        // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                        typeName = "byte";
                    }

                    if (typeName.EndsWith('*') || typeName.Contains("delegate*", StringComparison.Ordinal))
                    {
                        if (Config.GenerateGenericPointerWrapper)
                        {
                            AddDiagnostic(DiagnosticLevel.Warning, $"Unhandled pointer in template: '{typeName}'. Falling back 'IntPtr'.", cursor);
                        }

                        // Pointers are not yet supported as generic arguments; remap to IntPtr
                        typeName = "IntPtr";
                        _outputBuilder?.EmitSystemSupport();
                    }

                    _ = nameBuilder.Append(typeName);

                    shouldWritePrecedingComma = true;
                }

                if (!isStdAtomic)
                {
                    _ = nameBuilder.Append('>');
                }

                result.typeName = nameBuilder.ToString();
            }
            else if (type is TemplateTypeParmType templateTypeParmType)
            {
                if (templateTypeParmType.IsSugared)
                {
                    result.typeName = GetTypeName(cursor, context, rootType, templateTypeParmType.Desugar, ignoreTransparentStructsWhereRequired, isTemplate, out _);
                }
                else
                {
                    // The default name should be correct
                }
            }
            else if (type is TypedefType typedefType)
            {
                // We check remapped names here so that types that have variable sizes
                // can be treated correctly. Otherwise, they will resolve to a particular
                // platform size, based on whatever parameters were passed into clang.

                var remappedName = GetRemappedName(result.typeName, cursor, tryRemapOperatorName: false, out var wasRemapped, skipUsing: true);
                result.typeName = wasRemapped ? remappedName : GetTypeName(cursor, context, rootType, typedefType.Decl.UnderlyingType, ignoreTransparentStructsWhereRequired, isTemplate, out _);
            }
            else if (type is UsingType usingType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, usingType.Desugar, ignoreTransparentStructsWhereRequired, isTemplate, out _);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.TypeClass}'. Falling back '{result.typeName}'.", cursor);
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(result.typeName));
            Debug.Assert(!string.IsNullOrWhiteSpace(result.nativeTypeName));

            if (IsNativeTypeNameEquivalent(result.nativeTypeName, result.typeName))
            {
                result.nativeTypeName = string.Empty;
            }

            _typeNames[(cursor, context, type)] = result;
        }

        nativeTypeName = result.nativeTypeName;
        return result.typeName;
    }

    private string GetTypeNameForPointeeType(Cursor? cursor, Cursor? context, Type rootType, Type pointeeType, bool ignoreTransparentStructsWhereRequired, bool isTemplate, out string nativePointeeTypeName, out bool isAdjusted)
    {
        var name = pointeeType.AsString;

        nativePointeeTypeName = name;
        isAdjusted = false;

        // We don't want to handle these using IsType because we need to specially
        // handle cases like TypedefType at each level of the type hierarchy

        if (pointeeType is AttributedType attributedType)
        {
            name = GetTypeNameForPointeeType(cursor, context, rootType, attributedType.ModifiedType, ignoreTransparentStructsWhereRequired, isTemplate, out var nativeModifiedTypeName, out isAdjusted);
        }
        else if (pointeeType is ElaboratedType elaboratedType)
        {
            name = GetTypeNameForPointeeType(cursor, context, rootType, elaboratedType.NamedType, ignoreTransparentStructsWhereRequired, isTemplate, out var nativeNamedTypeName, out isAdjusted);

            if (!string.IsNullOrWhiteSpace(nativeNamedTypeName) &&
                !nativePointeeTypeName.StartsWith("const ", StringComparison.Ordinal) &&
                !nativePointeeTypeName.StartsWith("enum ", StringComparison.Ordinal) &&
                !nativePointeeTypeName.StartsWith("struct ", StringComparison.Ordinal) &&
                !nativePointeeTypeName.StartsWith("union ", StringComparison.Ordinal))
            {
                nativePointeeTypeName = nativeNamedTypeName;
                isAdjusted = true;
            }
        }
        else if (pointeeType is FunctionType functionType)
        {
            if (!_config.ExcludeFnptrCodegen && IsType<FunctionProtoType>(cursor, functionType, out var functionProtoType))
            {
                _config.ExcludeFnptrCodegen = true;
                var callConv = GetCallingConvention(cursor, context, rootType);
                _config.ExcludeFnptrCodegen = false;

                var needsReturnFixup = false;
                var returnTypeName = GetRemappedTypeName(cursor, context: null, functionType.ReturnType, out _, skipUsing: true);

                if (!_config.GenerateDisableRuntimeMarshalling && returnTypeName.Equals("bool", StringComparison.Ordinal))
                {
                    // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                    returnTypeName = "byte";
                }

                var nameBuilder = new StringBuilder();
                _ = nameBuilder.Append("delegate");
                _ = nameBuilder.Append('*');

                var isMacroDefinitionRecord = (cursor is VarDecl varDecl) && GetCursorName(varDecl).StartsWith("ClangSharpMacro_", StringComparison.Ordinal);

                if (!isMacroDefinitionRecord)
                {
                    _ = nameBuilder.Append(" unmanaged");
                    var hasSuppressGCTransition = HasSuppressGCTransition(cursor);

                    if (callConv != CallConv.Winapi)
                    {
                        _ = nameBuilder.Append('[');
                        _ = nameBuilder.Append(callConv.AsString(true));

                        if (hasSuppressGCTransition)
                        {
                            _ = nameBuilder.Append(", SuppressGCTransition");
                        }
                        _ = nameBuilder.Append(']');
                    }
                    else if (hasSuppressGCTransition)
                    {
                        _ = nameBuilder.Append("[SuppressGCTransition]");
                    }
                }

                _ = nameBuilder.Append('<');

                if ((cursor is CXXMethodDecl cxxMethodDecl) && (context is CXXRecordDecl cxxRecordDecl))
                {
                    var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
                    needsReturnFixup = cxxMethodDecl.IsVirtual && NeedsReturnFixup(cxxMethodDecl);

                    _ = nameBuilder.Append(EscapeName(cxxRecordDeclName));
                    _ = nameBuilder.Append('*');
                    _ = nameBuilder.Append(',');
                    _ = nameBuilder.Append(' ');

                    if (needsReturnFixup)
                    {
                        _ = nameBuilder.Append(returnTypeName);
                        _ = nameBuilder.Append('*');
                        _ = nameBuilder.Append(',');
                        _ = nameBuilder.Append(' ');
                    }
                }

                IEnumerable<Type> paramTypes = functionProtoType.ParamTypes;

                if (isMacroDefinitionRecord)
                {
                    Debug.Assert(cursor is not null);
                    varDecl = (VarDecl)cursor;

                    if (IsStmtAsWritten<DeclRefExpr>(varDecl.Init, out var declRefExpr, removeParens: true) && (declRefExpr.Decl is FunctionDecl functionDecl))
                    {
                        cursor = functionDecl;
                        paramTypes = functionDecl.Parameters.Select((param) => param.Type);
                        returnTypeName = GetRemappedTypeName(cursor, context: null, functionDecl.ReturnType, out _, skipUsing: true);
                    }
                }

                foreach (var paramType in paramTypes)
                {
                    var typeName = GetRemappedTypeName(cursor, context: null, paramType, out _, skipUsing: true);

                    if (!_config.GenerateDisableRuntimeMarshalling && typeName.Equals("bool", StringComparison.Ordinal))
                    {
                        // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                        typeName = "byte";
                    }

                    _ = nameBuilder.Append(typeName);
                    _ = nameBuilder.Append(',');
                    _ = nameBuilder.Append(' ');
                }

                if (!needsReturnFixup && ignoreTransparentStructsWhereRequired && _config.WithTransparentStructs.TryGetValue(returnTypeName, out var transparentStruct))
                {
                    _ = nameBuilder.Append(transparentStruct.Name);
                }
                else
                {
                    _ = nameBuilder.Append(returnTypeName);

                    if (needsReturnFixup)
                    {
                        _ = nameBuilder.Append('*');
                    }
                }

                _ = nameBuilder.Append('>');
                name = nameBuilder.ToString();
            }
            else
            {
                name = "IntPtr";
            }
        }
        else if (pointeeType is TypedefType typedefType)
        {
            // We check remapped names here so that types that have variable sizes
            // can be treated correctly. Otherwise, they will resolve to a particular
            // platform size, based on whatever parameters were passed into clang.

            var remappedName = GetRemappedName(name, cursor, tryRemapOperatorName: false, out var wasRemapped, skipUsing: true);

            if (wasRemapped)
            {
                name = isTemplate && Config.GenerateGenericPointerWrapper
                     ? $"Pointer<{remappedName}>"
                     : $"{remappedName}*";
            }
            else
            {
                name = GetTypeNameForPointeeType(cursor, context, rootType, typedefType.Decl.UnderlyingType, ignoreTransparentStructsWhereRequired, isTemplate, out var nativeUnderlyingTypeName, out isAdjusted);
            }
        }
        else
        {
            // Otherwise fields that point at anonymous structs get the wrong name
            var remappedName = GetRemappedTypeName(cursor, context, pointeeType, out nativePointeeTypeName, skipUsing: true);

            name = isTemplate && Config.GenerateGenericPointerWrapper
                 ? $"Pointer<{remappedName}>"
                 : $"{remappedName}*";
        }

        return name;
    }

    private void GetTypeSize(Cursor cursor, Type type, ref long alignment32, ref long alignment64, out long size32, out long size64)
    {
        var has8BytePrimitiveField = false;
        GetTypeSize(cursor, type, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
    }

    private void GetTypeSize(Cursor cursor, Type type, ref long alignment32, ref long alignment64, ref bool has8BytePrimitiveField, out long size32, out long size64)
    {
        size32 = 0;
        size64 = 0;

        // We don't want to handle these using IsType because we need to specially
        // handle cases like TypedefType at each level of the type hierarchy

        if (type is ArrayType arrayType)
        {
            if (IsTypeConstantOrIncompleteArray(cursor, type))
            {
                var count = Math.Max((arrayType as ConstantArrayType)?.Size ?? 0, 1);
                GetTypeSize(cursor, arrayType.ElementType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out var elementSize32, out var elementSize64);

                size32 = elementSize32 * Math.Max(count, 1);
                size64 = elementSize64 * Math.Max(count, 1);

                if (alignment32 == -1)
                {
                    alignment32 = elementSize32;
                }

                if (alignment64 == -1)
                {
                    alignment64 = elementSize64;
                }
            }
            else
            {
                size32 = 4;
                size64 = 8;

                if (alignment32 == -1)
                {
                    alignment32 = 4;
                }

                if (alignment64 == -1)
                {
                    alignment64 = 8;
                }
            }
        }
        else if (type is AttributedType attributedType)
        {
            GetTypeSize(cursor, attributedType.ModifiedType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is BuiltinType)
        {
            switch (type.Kind)
            {
                case CXType_Bool:
                case CXType_Char_U:
                case CXType_UChar:
                case CXType_Char_S:
                case CXType_SChar:
                {
                    size32 = 1;
                    size64 = 1;
                    break;
                }

                case CXType_UShort:
                case CXType_Short:
                {
                    size32 = 2;
                    size64 = 2;
                    break;
                }

                case CXType_UInt:
                case CXType_Int:
                case CXType_Float:
                {
                    size32 = 4;
                    size64 = 4;
                    break;
                }

                case CXType_ULong:
                case CXType_Long:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        size32 = 4;
                        size64 = 8;

                        if (alignment32 == -1)
                        {
                            alignment32 = 4;
                        }

                        if (alignment64 == -1)
                        {
                            alignment64 = 8;
                        }
                    }
                    else
                    {
                        goto case CXType_UInt;
                    }
                    break;
                }

                case CXType_ULongLong:
                case CXType_LongLong:
                case CXType_Double:
                {
                    size32 = 8;
                    size64 = 8;

                    if (alignment32 == -1)
                    {
                        alignment32 = 8;
                    }

                    if (alignment64 == -1)
                    {
                        alignment64 = 8;
                    }

                    has8BytePrimitiveField = true;
                    break;
                }

                case CXType_WChar:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        goto case CXType_Int;
                    }
                    else
                    {
                        goto case CXType_UShort;
                    }
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported builtin type: '{type.KindSpelling}.", cursor);
                    break;
                }
            }
        }
        else if (type is DecltypeType decltypeType)
        {
            GetTypeSize(cursor, decltypeType.UnderlyingType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is ElaboratedType elaboratedType)
        {
            GetTypeSize(cursor, elaboratedType.NamedType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is EnumType enumType)
        {
            GetTypeSize(cursor, enumType.Decl.IntegerType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is FunctionType or PointerType or ReferenceType)
        {
            size32 = 4;
            size64 = 8;

            if (alignment32 == -1)
            {
                alignment32 = 4;
            }

            if (alignment64 == -1)
            {
                alignment64 = 8;
            }
        }
        else if (type is InjectedClassNameType)
        {
            // Nothing to handle
        }
        else if (type is RecordType recordType)
        {
            var recordTypeAlignOf = Math.Min(recordType.Handle.AlignOf, 8);

            if (alignment32 == -1)
            {
                alignment32 = recordTypeAlignOf;
            }

            if (alignment64 == -1)
            {
                alignment64 = recordTypeAlignOf;
            }

            long maxFieldAlignment32 = -1;
            long maxFieldAlignment64 = -1;

            long maxFieldSize32 = 0;
            long maxFieldSize64 = 0;

            var anyFieldIs8BytePrimitive = false;

            if (recordType.Decl is CXXRecordDecl cxxRecordDecl)
            {
                if (HasVtbl(cxxRecordDecl, out _))
                {
                    size32 += 4;
                    size64 += 8;

                    if (alignment32 < 4)
                    {
                        alignment32 = Math.Max(Math.Min(alignment32, 4), 1);
                    }

                    if (alignment64 < 4)
                    {
                        alignment64 = Math.Max(Math.Min(alignment64, 8), 1);
                    }

                    maxFieldSize32 = Math.Max(maxFieldSize32, 4);
                    maxFieldSize64 = Math.Max(maxFieldSize64, 8);

                    maxFieldAlignment32 = Math.Max(maxFieldSize32, 4);
                    maxFieldAlignment64 = Math.Max(maxFieldSize64, 8);
                }
                else
                {
                    foreach (var baseCXXRecordDecl in cxxRecordDecl.Bases)
                    {
                        long fieldAlignment32 = -1;
                        long fieldAlignment64 = -1;

                        GetTypeSize(baseCXXRecordDecl, baseCXXRecordDecl.Type, ref fieldAlignment32, ref fieldAlignment64, ref anyFieldIs8BytePrimitive, out var fieldSize32, out var fieldSize64);

                        if ((fieldAlignment32 == -1) || (alignment32 < 4))
                        {
                            fieldAlignment32 = Math.Max(Math.Min(alignment32, fieldSize32), 1);
                        }

                        if ((fieldAlignment64 == -1) || (alignment64 < 4))
                        {
                            fieldAlignment64 = Math.Max(Math.Min(alignment64, fieldSize64), 1);
                        }

                        if ((size32 % fieldAlignment32) != 0)
                        {
                            size32 += fieldAlignment32 - (size32 % fieldAlignment32);
                        }

                        if ((size64 % fieldAlignment64) != 0)
                        {
                            size64 += fieldAlignment64 - (size64 % fieldAlignment64);
                        }

                        size32 += fieldSize32;
                        size64 += fieldSize64;

                        maxFieldAlignment32 = Math.Max(maxFieldAlignment32, fieldAlignment32);
                        maxFieldAlignment64 = Math.Max(maxFieldAlignment64, fieldAlignment64);

                        maxFieldSize32 = Math.Max(maxFieldSize32, fieldSize32);
                        maxFieldSize64 = Math.Max(maxFieldSize64, fieldSize64);
                    }
                }
            }

            var bitfieldPreviousSize32 = 0L;
            var bitfieldPreviousSize64 = 0L;
            var bitfieldRemainingBits32 = 0L;
            var bitfieldRemainingBits64 = 0L;

            foreach (var fieldDecl in recordType.Decl.Fields)
            {
                long fieldAlignment32 = -1;
                long fieldAlignment64 = -1;

                GetTypeSize(fieldDecl, fieldDecl.Type, ref fieldAlignment32, ref fieldAlignment64, ref anyFieldIs8BytePrimitive, out var fieldSize32, out var fieldSize64);

                var ignoreFieldSize32 = false;
                var ignoreFieldSize64 = false;

                if (fieldDecl.IsBitField)
                {
                    if (fieldSize32 != bitfieldPreviousSize32)
                    {
                        bitfieldRemainingBits32 = fieldSize32 * 8;
                        bitfieldPreviousSize32 = fieldSize32;
                        bitfieldRemainingBits32 -= fieldDecl.BitWidthValue;
                    }
                    else if (fieldDecl.BitWidthValue > bitfieldRemainingBits32)
                    {
                        if (bitfieldRemainingBits32 != bitfieldRemainingBits64)
                        {
                            ignoreFieldSize32 = true;
                        }

                        bitfieldRemainingBits32 = fieldSize32 * 8;
                        bitfieldPreviousSize32 = fieldSize32;
                        bitfieldRemainingBits32 -= fieldDecl.BitWidthValue;
                    }
                    else
                    {
                        bitfieldPreviousSize32 = fieldSize32;
                        bitfieldRemainingBits32 -= fieldDecl.BitWidthValue;
                        ignoreFieldSize32 = true;
                    }

                    if ((fieldSize64 != bitfieldPreviousSize64) || (fieldDecl.BitWidthValue > bitfieldRemainingBits64))
                    {
                        bitfieldRemainingBits64 = fieldSize64 * 8;
                        bitfieldPreviousSize64 = fieldSize64;
                        bitfieldRemainingBits64 -= fieldDecl.BitWidthValue;
                    }
                    else
                    {
                        bitfieldPreviousSize64 = fieldSize64;
                        bitfieldRemainingBits64 -= fieldDecl.BitWidthValue;
                        ignoreFieldSize64 = true;
                    }
                }

                if (!ignoreFieldSize32)
                {
                    if ((fieldAlignment32 == -1) || (alignment32 < 4))
                    {
                        fieldAlignment32 = Math.Max(Math.Min(alignment32, fieldSize32), 1);
                    }

                    if ((size32 % fieldAlignment32) != 0)
                    {
                        size32 += fieldAlignment32 - (size32 % fieldAlignment32);
                    }

                    size32 += fieldSize32;
                    maxFieldAlignment32 = Math.Max(maxFieldAlignment32, fieldAlignment32);
                    maxFieldSize32 = Math.Max(maxFieldSize32, fieldSize32);
                }

                if (!ignoreFieldSize64)
                {
                    if ((fieldAlignment64 == -1) || (alignment64 < 4))
                    {
                        fieldAlignment64 = Math.Max(Math.Min(alignment64, fieldSize64), 1);
                    }

                    if ((size64 % fieldAlignment64) != 0)
                    {
                        size64 += fieldAlignment64 - (size64 % fieldAlignment64);
                    }

                    size64 += fieldSize64;
                    maxFieldAlignment64 = Math.Max(maxFieldAlignment64, fieldAlignment64);
                    maxFieldSize64 = Math.Max(maxFieldSize64, fieldSize64);
                }
            }

            if ((alignment32 == 8) && !anyFieldIs8BytePrimitive)
            {
                alignment32 = Math.Min(alignment32, maxFieldAlignment32);
            }

            if ((alignment64 == 4) && !anyFieldIs8BytePrimitive)
            {
                alignment64 = Math.Max(alignment64, maxFieldAlignment64);
            }

            if (recordType.Decl.IsUnion)
            {
                size32 = maxFieldSize32;
                size64 = maxFieldSize64;
            }

            if ((size32 % alignment32) != 0)
            {
                size32 += alignment32 - (size32 % alignment32);
            }

            if ((size64 % alignment64) != 0)
            {
                size64 += alignment64 - (size64 % alignment64);
            }

            has8BytePrimitiveField |= anyFieldIs8BytePrimitive;
        }
        else if (type is TypedefType typedefType)
        {
            // We check remapped names here so that types that have variable sizes
            // can be treated correctly. Otherwise, they will resolve to a particular
            // platform size, based on whatever parameters were passed into clang.

            var name = GetTypeName(cursor, context: null, type: type, ignoreTransparentStructsWhereRequired: false, isTemplate: false, nativeTypeName: out _);
            var remappedName = GetRemappedTypeName(cursor, context: null, type, out _, skipUsing: true, ignoreTransparentStructsWhereRequired: false);

            if ((remappedName == name) && _config.WithTransparentStructs.TryGetValue(remappedName, out var transparentStruct) && (transparentStruct.Name.Equals("long", StringComparison.Ordinal) || transparentStruct.Name.Equals("ulong", StringComparison.Ordinal)))
            {
                size32 = 8;
                size64 = 8;

                if (alignment32 == -1)
                {
                    alignment32 = 8;
                }

                if (alignment64 == -1)
                {
                    alignment64 = 8;
                }

                has8BytePrimitiveField = true;
            }
            else if (remappedName.Equals("IntPtr", StringComparison.Ordinal) ||
                     remappedName.Equals("nint", StringComparison.Ordinal) ||
                     remappedName.Equals("nuint", StringComparison.Ordinal) ||
                     remappedName.Equals("UIntPtr", StringComparison.Ordinal) ||
                     remappedName.EndsWith('*'))
            {
                size32 = 4;
                size64 = 8;

                if (alignment32 == -1)
                {
                    alignment32 = 4;
                }

                if (alignment64 == -1)
                {
                    alignment64 = 8;
                }
            }
            else
            {
                GetTypeSize(cursor, typedefType.Decl.UnderlyingType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
            }
        }
        else if (type is SubstTemplateTypeParmType substTemplateTypeParmType)
        {
            GetTypeSize(cursor, substTemplateTypeParmType.ReplacementType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is TemplateSpecializationType templateSpecializationType)
        {
            if (templateSpecializationType.IsTypeAlias)
            {
                GetTypeSize(cursor, templateSpecializationType.AliasedType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
            }
            else if (templateSpecializationType.IsSugared)
            {
                GetTypeSize(cursor, templateSpecializationType.Desugar, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
            }
            else if (templateSpecializationType.TemplateName.AsTemplateDecl is TemplateDecl templateDecl)
            {
                if (templateDecl.TemplatedDecl is TypeDecl typeDecl)
                {
                    GetTypeSize(cursor, typeDecl.TypeForDecl, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported template specialization declaration kind: '{templateDecl.TemplatedDecl.DeclKindName}'.", cursor);
                }
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported template specialization type: '{templateSpecializationType}'.", cursor);
            }
        }
        else if (type is TemplateTypeParmType)
        {
            // Nothing to handle
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported type: '{type.TypeClass}'.", cursor);
        }
    }
}
