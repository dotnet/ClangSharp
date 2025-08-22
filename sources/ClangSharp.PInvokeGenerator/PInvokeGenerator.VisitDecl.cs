// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using static ClangSharp.Interop.CX_CastKind;
using static ClangSharp.Interop.CX_CharacterKind;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CX_StorageClass;
using static ClangSharp.Interop.CX_StringKind;
using static ClangSharp.Interop.CX_UnaryExprOrTypeTrait;
using static ClangSharp.Interop.CXUnaryOperatorKind;
using static ClangSharp.Interop.CXEvalResultKind;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public partial class PInvokeGenerator
{
    private void VisitClassTemplateDecl(ClassTemplateDecl classTemplateDecl) => Visit(classTemplateDecl.TemplatedDecl);

    private void VisitClassTemplateSpecializationDecl(ClassTemplateSpecializationDecl classTemplateSpecializationDecl) => AddDiagnostic(DiagnosticLevel.Warning, $"Class template specializations are not supported: '{GetCursorQualifiedName(classTemplateSpecializationDecl)}'. Generated bindings may be incomplete.", classTemplateSpecializationDecl);

    private void VisitDecl(Decl decl)
    {
        if (IsExcluded(decl))
        {
            if (decl.Kind == CX_DeclKind_Typedef)
            {
                VisitTypedefDecl((TypedefDecl)decl, onlyHandleRemappings: true);
            }
            return;
        }

        switch (decl.Kind)
        {
            case CX_DeclKind_AccessSpec:
            {
                // Access specifications are also exposed as a queryable property
                // on the declarations they impact, so we don't need to do anything
                break;
            }

            // case CX_DeclKind_Block:
            // case CX_DeclKind_Captured:
            // case CX_DeclKind_ClassScopeFunctionSpecialization:

            case CX_DeclKind_Empty:
            {
                // Nothing to generate for empty declarations
                break;
            }

            // case CX_DeclKind_Export:
            // case CX_DeclKind_ExternCContext:
            // case CX_DeclKind_FileScopeAsm:

            case CX_DeclKind_Friend:
            {
                // Nothing to generate for friend declarations
                break;
            }

            // case CX_DeclKind_FriendTemplate:
            // case CX_DeclKind_Import:

            case CX_DeclKind_LinkageSpec:
            {
                VisitLinkageSpecDecl((LinkageSpecDecl)decl);
                break;
            }

            case CX_DeclKind_Label:
            {
                VisitLabelDecl((LabelDecl)decl);
                break;
            }

            case CX_DeclKind_Namespace:
            {
                VisitNamespaceDecl((NamespaceDecl)decl);
                break;
            }

            // case CX_DeclKind_NamespaceAlias:
            // case CX_DeclKind_ObjCCompatibleAlias:
            // case CX_DeclKind_ObjCCategory:
            // case CX_DeclKind_ObjCCategoryImpl:
            // case CX_DeclKind_ObjCImplementation:
            // case CX_DeclKind_ObjCInterface:
            // case CX_DeclKind_ObjCProtocol:
            // case CX_DeclKind_ObjCMethod:
            // case CX_DeclKind_ObjCProperty:
            // case CX_DeclKind_BuiltinTemplate:
            // case CX_DeclKind_Concept:

            case CX_DeclKind_ClassTemplate:
            {
                VisitClassTemplateDecl((ClassTemplateDecl)decl);
                break;
            }

            case CX_DeclKind_FunctionTemplate:
            {
                VisitFunctionTemplateDecl((FunctionTemplateDecl)decl);
                break;
            }

            // case CX_DeclKind_TypeAliasTemplate:
            // case CX_DeclKind_VarTemplate:
            // case CX_DeclKind_TemplateTemplateParm:

            case CX_DeclKind_Enum:
            {
                VisitEnumDecl((EnumDecl)decl);
                break;
            }

            case CX_DeclKind_Record:
            case CX_DeclKind_CXXRecord:
            {
                VisitRecordDecl((RecordDecl)decl);
                break;
            }

            case CX_DeclKind_ClassTemplateSpecialization:
            {
                VisitClassTemplateSpecializationDecl((ClassTemplateSpecializationDecl)decl);
                break;
            }

            // case CX_DeclKind_ClassTemplatePartialSpecialization:
            // case CX_DeclKind_TemplateTypeParm:
            // case CX_DeclKind_ObjCTypeParam:

            case CX_DeclKind_TypeAlias:
            {
                // Nothing to generate for type alias declarations
                break;
            }

            case CX_DeclKind_Typedef:
            {
                VisitTypedefDecl((TypedefDecl)decl, onlyHandleRemappings: false);
                break;
            }

            // case CX_DeclKind_UnresolvedUsingTypename:

            case CX_DeclKind_Using:
            {
                // Using declarations only introduce existing members into
                // the current scope. There isn't an easy way to translate
                // this to C#, so we will ignore them for now.
                break;
            }

            // case CX_DeclKind_UsingDirective:
            // case CX_DeclKind_UsingPack:

            case CX_DeclKind_UsingShadow:
            {
                VisitUsingShadowDecl((UsingShadowDecl)decl);
                break;
            }

            // case CX_DeclKind_ConstructorUsingShadow:
            // case CX_DeclKind_Binding:

            case CX_DeclKind_Field:
            {
                VisitFieldDecl((FieldDecl)decl);
                break;
            }

            // case CX_DeclKind_ObjCAtDefsField:
            // case CX_DeclKind_ObjCIvar:

            case CX_DeclKind_Function:
            case CX_DeclKind_CXXMethod:
            case CX_DeclKind_CXXConstructor:
            case CX_DeclKind_CXXDestructor:
            case CX_DeclKind_CXXConversion:
            {
                VisitFunctionDecl((FunctionDecl)decl);
                break;
            }

            // case CX_DeclKind_CXXDeductionGuide:
            // case CX_DeclKind_MSProperty:
            // case CX_DeclKind_NonTypeTemplateParm:

            case CX_DeclKind_Var:
            {
                VisitVarDecl((VarDecl)decl);
                break;
            }

            // case CX_DeclKind_Decomposition:
            // case CX_DeclKind_ImplicitParam:
            // case CX_DeclKind_OMPCapturedExpr:

            case CX_DeclKind_ParmVar:
            {
                VisitParmVarDecl((ParmVarDecl)decl);
                break;
            }

            // case CX_DeclKind_VarTemplateSpecialization:
            // case CX_DeclKind_VarTemplatePartialSpecialization:

            case CX_DeclKind_EnumConstant:
            {
                VisitEnumConstantDecl((EnumConstantDecl)decl);
                break;
            }

            case CX_DeclKind_IndirectField:
            {
                VisitIndirectFieldDecl((IndirectFieldDecl)decl);
                break;
            }

            // case CX_DeclKind_OMPDeclareMapper:
            // case CX_DeclKind_OMPDeclareReduction:
            // case CX_DeclKind_UnresolvedUsingValue:
            // case CX_DeclKind_OMPAllocate:
            // case CX_DeclKind_OMPRequires:
            // case CX_DeclKind_OMPThreadPrivate:
            // case CX_DeclKind_ObjCPropertyImpl:

            case CX_DeclKind_PragmaComment:
            {
                // Pragma comments can't be easily modeled in C#
                // We'll ignore them for now.
                break;
            }

            // case CX_DeclKind_PragmaDetectMismatch:

            case CX_DeclKind_StaticAssert:
            {
                // Static asserts can't be easily modeled in C#
                // We'll ignore them for now.
                break;
            }

            case CX_DeclKind_TranslationUnit:
            {
                VisitTranslationUnitDecl((TranslationUnitDecl)decl);
                break;
            }

            default:
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported declaration: '{decl.DeclKindName}'. Generated bindings may be incomplete.", decl);
                break;
            }
        }
    }

    private void VisitEnumConstantDecl(EnumConstantDecl enumConstantDecl)
    {
        Debug.Assert(_outputBuilder is not null);

        var accessSpecifier = AccessSpecifier.None;
        var name = GetRemappedCursorName(enumConstantDecl);
        var typeName = GetTargetTypeName(enumConstantDecl, out _);
        var isAnonymousEnum = false;
        var parentName = "";

        if (enumConstantDecl.DeclContext is EnumDecl enumDecl)
        {
            parentName = GetRemappedCursorName(enumDecl);

            if (parentName.StartsWith("__AnonymousEnum_", StringComparison.Ordinal))
            {
                parentName = "";
                isAnonymousEnum = true;
                accessSpecifier = GetAccessSpecifier(enumDecl, matchStar: true);
            }
        }

        if (string.IsNullOrEmpty(parentName))
        {
            parentName = _outputBuilder.Name;
        }

        var escapedName = EscapeAndStripEnumMemberName(name, parentName);

        var kind = isAnonymousEnum ? ValueKind.Primitive : ValueKind.Enumerator;
        var flags = ValueFlags.Constant;

        if ((enumConstantDecl.InitExpr is not null) || isAnonymousEnum)
        {
            flags |= ValueFlags.Initializer;
        }

        var desc = new ValueDesc {
            AccessSpecifier = accessSpecifier,
            TypeName = typeName,
            EscapedName = escapedName,
            NativeTypeName = null,
            ParentName = parentName,
            Kind = kind,
            Flags = flags,
            Location = enumConstantDecl.Location,
            WriteCustomAttrs = static context => {
                (var enumConstantDecl, var generator) = ((EnumConstantDecl, PInvokeGenerator))context;

                generator.WithAttributes(enumConstantDecl);
                generator.WithUsings(enumConstantDecl);
            },
            CustomAttrGeneratorData = (enumConstantDecl, this),
        };

        _outputBuilder.BeginValue(in desc);

        if (enumConstantDecl.InitExpr != null)
        {
            Visit(enumConstantDecl.InitExpr);
        }
        else if (isAnonymousEnum)
        {
            if (IsUnsigned(typeName))
            {
                _outputBuilder.WriteConstantValue(enumConstantDecl.UnsignedInitVal);
            }
            else
            {
                _outputBuilder.WriteConstantValue(enumConstantDecl.InitVal);
            }
        }

        _outputBuilder.EndValue(in desc);
    }

    private void VisitEnumDecl(EnumDecl enumDecl)
    {
        var accessSpecifier = GetAccessSpecifier(enumDecl, matchStar: true);
        var name = GetRemappedCursorName(enumDecl);
        var escapedName = EscapeName(name);
        var isAnonymousEnum = false;

        if (name.StartsWith("__AnonymousEnum_", StringComparison.Ordinal))
        {
            isAnonymousEnum = true;

            if (!TryGetClass(name, out var className, disallowPrefixMatch: true))
            {
                className = _config.DefaultClass;
                _ = _topLevelClassNames.Add(className);
                _ = _topLevelClassNames.Add($"{className}Tests");
                AddDiagnostic(DiagnosticLevel.Info, $"Found anonymous enum: {name}. Mapping values as constants in: {className}", enumDecl);
            }

            name = className;
        }

        StartUsingOutputBuilder(name);
        {
            Debug.Assert(_outputBuilder is not null);
            EnumDesc desc = default;

            if (!isAnonymousEnum)
            {
                var typeName = GetRemappedTypeName(enumDecl, context: null, enumDecl.IntegerType, out var nativeTypeName);

                desc = new EnumDesc() {
                    AccessSpecifier = accessSpecifier,
                    TypeName = typeName,
                    EscapedName = escapedName,
                    NativeType = nativeTypeName,
                    Location = enumDecl.Location,
                    IsNested = enumDecl.DeclContext is TagDecl,
                    WriteCustomAttrs = static context => {
                        (var enumDecl, var generator) = ((EnumDecl, PInvokeGenerator))context;

                        generator.WithAttributes(enumDecl);
                        generator.WithUsings(enumDecl);
                    },
                    CustomAttrGeneratorData = (enumDecl, this),
                };

                _outputBuilder.BeginEnum(in desc);
            }

            Visit(enumDecl.Enumerators);
            Visit(enumDecl.Decls, excludedCursors: enumDecl.Enumerators);

            if (!isAnonymousEnum)
            {
                _outputBuilder.EndEnum(in desc);
            }
        }
        StopUsingOutputBuilder();
    }

    private void VisitFieldDecl(FieldDecl fieldDecl)
    {
        Debug.Assert(_outputBuilder is not null);

        if (fieldDecl.IsBitField)
        {
            return;
        }

        var accessSpecifier = GetAccessSpecifier(fieldDecl, matchStar: false);
        var name = GetRemappedCursorName(fieldDecl);
        var escapedName = EscapeName(name);

        var type = fieldDecl.Type;
        var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out var nativeTypeName);

        if (!_config.GenerateDisableRuntimeMarshalling && typeName.Equals("bool", StringComparison.Ordinal))
        {
            // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for structs that may be in P/Invoke signatures
            typeName = "byte";
            nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? "bool" : nativeTypeName;
        }

        if (_config.GenerateCompatibleCode && typeName.StartsWith("bool*", StringComparison.Ordinal))
        {
            // bool* is not blittable in compat mode, so we shouldn't use it for structs that may be in P/Invoke signatures
            typeName = typeName.Replace("bool*", "byte*", StringComparison.Ordinal);
            nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? typeName.Replace("byte*", "bool*", StringComparison.Ordinal) : nativeTypeName;
        }

        var parent = fieldDecl.Parent;
        Debug.Assert(parent is not null);

        int? offset = null;
        if (parent.IsUnion)
        {
            offset = 0;
        }

        var desc = new FieldDesc {
            AccessSpecifier = accessSpecifier,
            NativeTypeName = nativeTypeName,
            EscapedName = escapedName,
            ParentName = GetRemappedCursorName(parent),
            Offset = offset,
            NeedsNewKeyword = NeedsNewKeyword(name),
            Location = fieldDecl.Location,
            WriteCustomAttrs = static context => {
                (var fieldDecl, var generator) = ((FieldDecl, PInvokeGenerator))context;

                generator.WithAttributes(fieldDecl);
                generator.WithUsings(fieldDecl);
            },
            CustomAttrGeneratorData = (fieldDecl, this),
        };

        _outputBuilder.BeginField(in desc);

        if (IsTypeConstantOrIncompleteArray(fieldDecl, type, out var arrayType))
        {
            var count = Math.Max((arrayType as ConstantArrayType)?.Size ?? 0, 1).ToString(CultureInfo.InvariantCulture);
            var elementType = arrayType.ElementType;

            while (IsTypeConstantOrIncompleteArray(fieldDecl, elementType, out var subArrayType))
            {
                count += " * ";
                count += Math.Max((subArrayType as ConstantArrayType)?.Size ?? 0, 1).ToString(CultureInfo.InvariantCulture);
                elementType = subArrayType.ElementType;
            }

            _outputBuilder.WriteFixedCountField(typeName, escapedName, GetArtificialFixedSizedBufferName(fieldDecl), count);
        }
        else
        {
            _outputBuilder.WriteRegularField(typeName, escapedName);
        }

        _outputBuilder.EndField(in desc);
    }

    private void VisitFunctionDecl(FunctionDecl functionDecl)
    {
        if (!functionDecl.IsUserProvided)
        {
            // We shouldn't process injected functions
            return;
        }

        if (IsExcluded(functionDecl))
        {
            return;
        }

        var name = GetRemappedCursorName(functionDecl);

        var cxxMethodDecl = functionDecl as CXXMethodDecl;
        uint overloadCount = 0;

        if (cxxMethodDecl is not null and CXXConstructorDecl)
        {
            var parent = cxxMethodDecl.Parent;
            Debug.Assert(parent is not null);
            name = GetRemappedCursorName(parent);
        }

        if (cxxMethodDecl is not null)
        {
            overloadCount = GetOverloadCount(cxxMethodDecl);
        }

        var isManualImport = _config.WithManualImports.Contains(name);

        var className = name;
        var parentName = "";

        var cxxRecordDecl = functionDecl.DeclContext as CXXRecordDecl;

        if (cxxRecordDecl is null)
        {
            className = GetClass(name);
            parentName = className;
            StartUsingOutputBuilder(className);
        }
        else if ((Cursor?)functionDecl.LexicalDeclContext != cxxRecordDecl)
        {
            // We shouldn't reprocess C++ functions outside the declaration
            return;
        }

        var accessSpecifier = GetAccessSpecifier(functionDecl, matchStar: false);
        var body = functionDecl.Body;

        bool isVirtual;
        string escapedName;

        if ((cxxMethodDecl is not null) && cxxMethodDecl.IsVirtual)
        {
            isVirtual = true;
            escapedName = PrefixAndStripMethodName(name, GetOverloadIndex(cxxMethodDecl));
        }
        else
        {
            isVirtual = false;
            escapedName = EscapeAndStripMethodName(name);
        }

        var returnType = functionDecl.ReturnType;
        var returnTypeName = GetRemappedTypeName(functionDecl, cxxRecordDecl, returnType, out var nativeTypeName);

        if (isManualImport && !_config.WithClasses.ContainsKey(name))
        {
            var parameters = functionDecl.Parameters;
            var firstParameter = (parameters.Count != 0) ? parameters[0] : null;
            var firstParameterTypeName = (firstParameter is not null) ? GetTargetTypeName(firstParameter, out var _) : "void";
            AddDiagnostic(DiagnosticLevel.Warning, $"Found manual import for {name} with no class remapping. First Parameter Type: {firstParameterTypeName}; Return Type: {returnTypeName}", functionDecl);
        }

        if (isVirtual || (body is null))
        {
            if (!_config.GenerateDisableRuntimeMarshalling && returnTypeName.Equals("bool", StringComparison.Ordinal))
            {
                // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                returnTypeName = "byte";
                nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? "bool" : nativeTypeName;
            }

            if (_config.GenerateCompatibleCode && returnTypeName.StartsWith("bool*", StringComparison.Ordinal))
            {
                // bool* is not blittable in compat mode, so we shouldn't use it for P/Invoke signatures
                returnTypeName = returnTypeName.Replace("bool*", "byte*", StringComparison.Ordinal);
                nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? returnTypeName.Replace("byte*", "bool*", StringComparison.Ordinal) : nativeTypeName;
            }
        }

        var type = functionDecl.Type;
        var callingConventionName = GetCallingConvention(functionDecl, cxxRecordDecl, type);

        var isDllImport = (body is null) && !isVirtual;
        var entryPoint = "";

        if (isDllImport)
        {
            entryPoint = functionDecl.IsExternC ? GetCursorName(functionDecl) : functionDecl.Handle.Mangling.CString;
        }

        var needsReturnFixup = (cxxMethodDecl is not null) && NeedsReturnFixup(cxxMethodDecl);

        var desc = new FunctionOrDelegateDesc {
            AccessSpecifier = accessSpecifier,
            NativeTypeName = nativeTypeName,
            EscapedName = escapedName,
            ParentName = parentName,
            EntryPoint = entryPoint,
            CallingConvention = callingConventionName,
            LibraryPath = isDllImport ? GetLibraryPath(name).Unquote() : null,
            IsVirtual = isVirtual,
            IsDllImport = isDllImport,
            IsManualImport = isManualImport,
            HasFnPtrCodeGen = !_config.ExcludeFnptrCodegen,
            SetLastError = GetSetLastError(functionDecl),
            IsCxx = cxxMethodDecl is not null,
            IsStatic = isDllImport || (cxxMethodDecl is null) || cxxMethodDecl.IsStatic,
            NeedsNewKeyword = NeedsNewKeyword(escapedName, functionDecl.Parameters),
            IsUnsafe = IsUnsafe(functionDecl),
            IsCtxCxxRecord = cxxRecordDecl is not null,
            IsCxxRecordCtxUnsafe = cxxRecordDecl is not null && IsUnsafe(cxxRecordDecl),
            NeedsReturnFixup = needsReturnFixup,
            ReturnType = needsReturnFixup ? $"{returnTypeName}*" : returnTypeName,
            IsCxxConstructor = functionDecl is CXXConstructorDecl,
            Location = functionDecl.Location,
            HasBody = body is not null,
            WriteCustomAttrs = static context => {
                (var functionDecl, var outputBuilder, var generator) = ((FunctionDecl, IOutputBuilder, PInvokeGenerator))context;

                generator.WithAttributes(functionDecl);
                generator.WithUsings(functionDecl);

                if (generator.HasSuppressGCTransition(functionDecl))
                {
                    outputBuilder.WriteCustomAttribute("SuppressGCTransition");
                }
            },
            CustomAttrGeneratorData = (functionDecl, _outputBuilder, this),
            ParameterTypes = overloadCount > 1 ? [.. functionDecl.Parameters.Select(param => GetTargetTypeName(param, out var _))] : null,
        };
        Debug.Assert(_outputBuilder is not null);

        _ = _topLevelClassIsUnsafe.TryGetValue(className, out var isUnsafe);
        _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);
        _topLevelClassIsUnsafe[className] = isUnsafe;

        _outputBuilder.BeginFunctionInnerPrototype(in desc);

        var needsThis = isVirtual || ((cxxMethodDecl is not null) && (body is null) && cxxMethodDecl.IsInstance);

        if (needsThis)
        {
            Debug.Assert(cxxRecordDecl is not null);

            if (!IsPrevContextDecl<CXXRecordDecl>(out var thisCursor, out _))
            {
                thisCursor = cxxRecordDecl;
            }

            var cxxRecordDeclName = GetRemappedCursorName(thisCursor);
            var cxxRecordEscapedName = EscapeName(cxxRecordDeclName);
            var parameterDesc = new ParameterDesc {
                Name = "pThis",
                Type = $"{cxxRecordEscapedName}*",
            };

            _outputBuilder.BeginParameter(in parameterDesc);
            _outputBuilder.EndParameter(in parameterDesc);

            if (needsReturnFixup)
            {
                _outputBuilder.WriteParameterSeparator();
                parameterDesc = new() {
                    Name = "_result",
                    Type = $"{returnTypeName}*"
                };
                _outputBuilder.BeginParameter(in parameterDesc);
                _outputBuilder.EndParameter(in parameterDesc);
            }

            if (functionDecl.Parameters.Any())
            {
                _outputBuilder.WriteParameterSeparator();
            }
        }

        Visit(functionDecl.Parameters);

        if (functionDecl.IsVariadic)
        {
            if (needsThis || functionDecl.Parameters.Any())
            {
                _outputBuilder.WriteParameterSeparator();
            }
            var parameterDesc = new ParameterDesc {
                Name = "",
                Type = "__arglist"
            };
            _outputBuilder.BeginParameter(in parameterDesc);
            _outputBuilder.EndParameter(in parameterDesc);
        }

        _outputBuilder.EndFunctionInnerPrototype(in desc);

        if ((body is not null) && !isVirtual)
        {
            _outputBuilder.BeginBody();

            if ((_cxxRecordDeclContext is not null) && (cxxRecordDecl is not null) && (_cxxRecordDeclContext != cxxRecordDecl) && HasField(cxxRecordDecl))
            {
                Debug.Assert(cxxMethodDecl is not null);

                var outputBuilder = StartCSharpCode();
                outputBuilder.WriteIndentation();

                if (!IsTypeVoid(functionDecl, returnType))
                {
                    outputBuilder.Write("return ");
                }

                var parent = cxxMethodDecl.Parent;
                Debug.Assert(parent is not null);

                var cxxBaseSpecifier = _cxxRecordDeclContext.Bases.Where((baseSpecifier) => baseSpecifier.Referenced == parent).SingleOrDefault();

                if (cxxBaseSpecifier is not null)
                {
                    var baseFieldName = GetAnonymousName(cxxBaseSpecifier, "Base");
                    baseFieldName = GetRemappedName(baseFieldName, cxxBaseSpecifier, tryRemapOperatorName: true, out var wasRemapped, skipUsing: true);
                    outputBuilder.Write(baseFieldName);
                }
                else
                {
                    outputBuilder.Write("Base");
                }

                outputBuilder.Write('.');
                outputBuilder.Write(name);
                outputBuilder.Write('(');

                var parameters = functionDecl.Parameters;

                if (parameters.Count != 0)
                {
                    var parameter = parameters[0];
                    var parameterName = GetRemappedCursorName(parameter);
                    outputBuilder.Write(EscapeName(parameterName));

                    for (var i = 1; i < parameters.Count; i++)
                    {
                        parameter = parameters[i];
                        parameterName = GetRemappedCursorName(parameter);

                        outputBuilder.Write(", ");
                        outputBuilder.Write(EscapeName(parameterName));
                    }
                }

                if (functionDecl.IsVariadic)
                {
                    if (parameters.Count != 0)
                    {
                        outputBuilder.Write(", ");
                    }
                    outputBuilder.Write("__arglist");
                }

                outputBuilder.Write(')');

                outputBuilder.NeedsSemicolon = true;
                outputBuilder.NeedsNewline = true;

                StopCSharpCode();
            }
            else
            {
                var firstCtorInitializer = functionDecl.Parameters.Any() ? (functionDecl.CursorChildren.IndexOf(functionDecl.Parameters[functionDecl.Parameters.Count - 1]) + 1) : 0;
                var lastCtorInitializer = (functionDecl.Body is not null) ? functionDecl.CursorChildren.IndexOf(functionDecl.Body) : functionDecl.CursorChildren.Count;

                if (functionDecl is CXXConstructorDecl cxxConstructorDecl)
                {
                    VisitCtorInitializers(cxxConstructorDecl, firstCtorInitializer, lastCtorInitializer);
                }

                if (body is CompoundStmt compoundStmt)
                {
                    var currentContext = _context.AddLast((compoundStmt, null));

                    _outputBuilder.BeginConstructorInitializers();
                    VisitStmts(compoundStmt.Body);
                    _outputBuilder.EndConstructorInitializers();

                    Debug.Assert(_context.Last == currentContext);
                    _context.RemoveLast();
                }
                else
                {
                    _outputBuilder.BeginInnerFunctionBody();
                    Visit(body);
                    _outputBuilder.EndInnerFunctionBody();
                }
            }

            _outputBuilder.EndBody();
        }

        _outputBuilder.EndFunctionOrDelegate(in desc);

        Visit(functionDecl.Decls, excludedCursors: functionDecl.Parameters);

        if (cxxRecordDecl is null)
        {
            StopUsingOutputBuilder();
        }

        void VisitCtorInitializers(CXXConstructorDecl cxxConstructorDecl, int firstCtorInitializer, int lastCtorInitializer)
        {
            for (var i = firstCtorInitializer; i < lastCtorInitializer; i++)
            {
                if (cxxConstructorDecl.CursorChildren[i] is Attr)
                {
                    continue;
                }
                if (cxxConstructorDecl.CursorChildren[i] is not Ref memberRef
                    || cxxConstructorDecl.CursorChildren[++i] is not Stmt memberInit)
                {
                    continue;
                }

                if (memberInit is ImplicitValueInitExpr)
                {
                    continue;
                }

                var memberRefName = GetRemappedCursorName(memberRef.Referenced);
                var memberInitName = memberInit.Spelling;

                if (memberInit is CastExpr { SubExprAsWritten: DeclRefExpr declRefExpr })
                {
                    memberInitName = GetRemappedCursorName(declRefExpr.Decl);
                }

                var skipInitializer = false;
                var typeName = "";

                if (memberInit is InitListExpr initListExpr)
                {
                    typeName = GetRemappedTypeName(initListExpr, context: null, initListExpr.Type, out _);
                }

                if (string.Equals(memberRefName, typeName, StringComparison.Ordinal))
                {
                    skipInitializer = true;
                }
                else
                {
                    _outputBuilder.BeginConstructorInitializer(memberRefName, memberInitName);
                }

                var memberRefTypeName = GetRemappedTypeName(memberRef, context: null, memberRef.Type, out var memberRefNativeTypeName);

                _ = _context.AddLast((cxxConstructorDecl, skipInitializer));
                UncheckStmt(memberRefTypeName, memberInit);
                _context.RemoveLast();

                if (!skipInitializer)
                {
                    _outputBuilder.EndConstructorInitializer();
                }

            }
        }
    }

    private void VisitFunctionTemplateDecl(FunctionTemplateDecl functionTemplateDecl) => Visit(functionTemplateDecl.TemplatedDecl);

    private void VisitIndirectFieldDecl(IndirectFieldDecl indirectFieldDecl)
    {
        if (_config.ExcludeAnonymousFieldHelpers)
        {
            return;
        }

        if (IsPrevContextDecl<RecordDecl>(out var prevContext, out _) && prevContext.IsAnonymous)
        {
            // We shouldn't process indirect fields where the prev context is an anonymous record decl
            return;
        }

        var fieldDecl = indirectFieldDecl.AnonField;

        var anonymousRecordDecl = fieldDecl.Parent;
        Debug.Assert(anonymousRecordDecl is not null);
        var rootRecordDecl = anonymousRecordDecl;

        var contextNameParts = new Stack<string>();
        var contextTypeParts = new Stack<string>();

        while (rootRecordDecl.IsAnonymous && (rootRecordDecl.Parent is RecordDecl parentRecordDecl))
        {
            // The name of a field of an anonymous type should be same as the type's name minus the
            // type kind tag at the end and the leading `_`.
            var contextNamePart = GetRemappedCursorName(rootRecordDecl);
            var tagIndex = contextNamePart.LastIndexOf("_e__", StringComparison.Ordinal);
            Debug.Assert(contextNamePart[0] == '_');
            Debug.Assert(tagIndex >= 0);
            contextNamePart = contextNamePart.Substring(1, tagIndex - 1);

            contextNameParts.Push(EscapeName(contextNamePart));

            contextTypeParts.Push(GetRemappedTypeName(rootRecordDecl, context: null, rootRecordDecl.TypeForDecl, out _));

            rootRecordDecl = parentRecordDecl;
        }

        var contextNameBuilder = new StringBuilder(contextNameParts.Pop());
        var contextTypeBuilder = new StringBuilder(contextTypeParts.Pop());

        while (contextNameParts.Count != 0)
        {
            _ = contextNameBuilder.Append('.');
            _ = contextNameBuilder.Append(contextNameParts.Pop());

            _ = contextTypeBuilder.Append('.');
            _ = contextTypeBuilder.Append(contextTypeParts.Pop());
        }

        var contextName = contextNameBuilder.ToString();
        var contextType = contextTypeBuilder.ToString();

        var type = fieldDecl.Type;

        var accessSpecifier = GetAccessSpecifier(anonymousRecordDecl, matchStar: true);

        var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out _);
        var name = GetRemappedCursorName(fieldDecl);
        var escapedName = EscapeName(name);

        var rootRecordDeclName = GetRemappedCursorName(rootRecordDecl);

        if (_config.ExcludedNames.Contains($"{rootRecordDeclName}.{name}") || _config.ExcludedNames.Contains($"{rootRecordDeclName}::{name}"))
        {
            return;
        }

        var parent = fieldDecl.Parent;
        Debug.Assert(parent is not null);

        var desc = new FieldDesc {
            AccessSpecifier = accessSpecifier,
            NativeTypeName = null,
            EscapedName = escapedName,
            ParentName = GetRemappedCursorName(parent),
            Offset = null,
            NeedsNewKeyword = false,
            NeedsUnscopedRef = _config.GenerateLatestCode && !fieldDecl.IsBitField,
            Location = fieldDecl.Location,
            HasBody = true,
            WriteCustomAttrs = static context => {
                (var fieldDecl, var generator) = ((FieldDecl, PInvokeGenerator))context;

                generator.WithAttributes(fieldDecl);
                generator.WithUsings(fieldDecl);
            },
            CustomAttrGeneratorData = (fieldDecl, this),
        };

        Debug.Assert(_outputBuilder is not null);

        _outputBuilder.WriteDivider(true);
        _outputBuilder.BeginField(in desc);

        var isFixedSizedBuffer = IsTypeConstantOrIncompleteArray(indirectFieldDecl, type);
        var generateCompatibleCode = _config.GenerateCompatibleCode;
        var typeStringBuilder = new StringBuilder();

        if (IsType<RecordType>(indirectFieldDecl, type, out var recordType))
        {
            var recordDecl = recordType.Decl;

            while ((recordDecl.DeclContext is RecordDecl parentRecordDecl) && (parentRecordDecl != rootRecordDecl))
            {
                var parentRecordDeclName = GetRemappedCursorName(parentRecordDecl);
                var escapedParentRecordDeclName = EscapeName(parentRecordDeclName);

                _ = typeStringBuilder.Insert(0, '.').Insert(0, escapedParentRecordDeclName);

                recordDecl = parentRecordDecl;
            }
        }

        if (!fieldDecl.IsBitField && (!isFixedSizedBuffer || generateCompatibleCode))
        {
            _ = typeStringBuilder.Insert(0, "ref ");
        }

        var isSupportedFixedSizedBufferType = isFixedSizedBuffer && IsSupportedFixedSizedBufferType(typeName);

        if (isFixedSizedBuffer)
        {
            if (!generateCompatibleCode)
            {
                _outputBuilder.EmitSystemSupport();
                _ = typeStringBuilder.Append("Span<");
            }
            else if (!isSupportedFixedSizedBufferType)
            {
                _ = typeStringBuilder.Append(contextType).Append('.');
                typeName = GetArtificialFixedSizedBufferName(fieldDecl);
            }
        }

        _ = typeStringBuilder.Append(typeName);
        if (isFixedSizedBuffer && !generateCompatibleCode)
        {
            _ = typeStringBuilder.Append('>');
        }

        var typeString = typeStringBuilder.ToString();
        _outputBuilder.WriteRegularField(typeString, escapedName);

        var isIndirectPointerField = IsTypePointerOrReference(indirectFieldDecl, type) && !typeName.Equals("IntPtr", StringComparison.Ordinal) && !typeName.Equals("UIntPtr", StringComparison.Ordinal);

        _outputBuilder.BeginBody();
        _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining, isReadOnly: fieldDecl.IsBitField && !Config.GenerateCompatibleCode);
        var code = _outputBuilder.BeginCSharpCode();

        if (fieldDecl.IsBitField)
        {
            code.WriteIndented("return ");
            code.Write(contextName);
            code.Write('.');
            code.Write(escapedName);
            code.WriteSemicolon();
            code.WriteNewline();
            _outputBuilder.EndCSharpCode(code);

            _outputBuilder.EndGetter();

            _outputBuilder.BeginSetter(_config.GenerateAggressiveInlining);

            code = _outputBuilder.BeginCSharpCode();
            code.WriteIndented(contextName);
            code.Write('.');
            code.Write(escapedName);
            code.Write(" = value");
            code.WriteSemicolon();
            code.WriteNewline();
            _outputBuilder.EndCSharpCode(code);

            _outputBuilder.EndSetter();
        }
        else if (generateCompatibleCode)
        {
            code.WriteIndented("fixed (");
            code.Write(contextType);
            code.Write("* pField = &");
            code.Write(contextName);
            code.WriteLine(')');
            code.WriteBlockStart();
            code.WriteIndented("return ref pField->");
            code.Write(escapedName);

            if (isSupportedFixedSizedBufferType)
            {
                code.Write("[0]");
            }

            code.WriteSemicolon();
            code.WriteNewline();
            code.WriteBlockEnd();
            _outputBuilder.EndCSharpCode(code);

            _outputBuilder.EndGetter();
        }
        else
        {
            code.WriteIndented("return ");

            if (desc.NeedsUnscopedRef && !isFixedSizedBuffer)
            {
                code.Write("ref ");
                code.Write(contextName);
                code.Write('.');
                code.Write(escapedName);
            }
            else
            {
                if (!isFixedSizedBuffer)
                {
                    code.AddUsingDirective("System.Runtime.InteropServices");
                    code.Write("ref MemoryMarshal.GetReference(");
                }

                if (!isFixedSizedBuffer || isSupportedFixedSizedBufferType)
                {
                    code.Write("MemoryMarshal.CreateSpan(ref ");
                }

                if (isIndirectPointerField)
                {
                    code.Write("this");
                }
                else
                {
                    code.Write(contextName);
                    code.Write('.');
                    code.Write(escapedName);
                }

                if (isFixedSizedBuffer)
                {
                    var arraySize = IsType<ConstantArrayType>(indirectFieldDecl, type, out var constantArrayType) ? constantArrayType.Size : 0;
                    arraySize = Math.Max(arraySize, 1);

                    if (isSupportedFixedSizedBufferType)
                    {
                        code.Write("[0], ");
                        code.Write(arraySize);
                        code.Write(')');
                    }
                    else if (!_config.GenerateLatestCode || arraySize == 1)
                    {
                        code.Write(".AsSpan(");

                        if (arraySize == 1)
                        {
                            if (TryGetRemappedValue(indirectFieldDecl, _config._withLengths, out var length))
                            {
                                code.Write(length);
                            }
                            else
                            {
                                AddDiagnostic(DiagnosticLevel.Warning, $"Found variable length array: '{GetCursorQualifiedName(indirectFieldDecl)}'. Please specify the length using `--with-length <string>`.", indirectFieldDecl);
                            }
                        }

                        code.Write(')');
                    }
                }
                else
                {
                    code.Write(", 1))");
                }

                if (isIndirectPointerField)
                {
                    code.Write('.');
                    code.Write(contextName);
                    code.Write('.');
                    code.Write(escapedName);
                }
            }

            code.WriteSemicolon();
            code.WriteNewline();
            _outputBuilder.EndCSharpCode(code);

            _outputBuilder.EndGetter();
        }

        _outputBuilder.EndBody();
        _outputBuilder.EndField(in desc);
        _outputBuilder.WriteDivider();
    }

    private static void VisitLabelDecl(LabelDecl labelDecl)
    {
        // This should have already been handled as a statement
    }

    private void VisitLinkageSpecDecl(LinkageSpecDecl linkageSpecDecl)
    {
        Visit(linkageSpecDecl.Decls);
        Visit(linkageSpecDecl.CursorChildren, linkageSpecDecl.Decls);
    }

    private void VisitNamespaceDecl(NamespaceDecl namespaceDecl)
    {
        // We don't currently include the namespace name anywhere in the
        // generated bindings. We might want to in the future...

        Visit(namespaceDecl.Decls);
        Visit(namespaceDecl.CursorChildren, namespaceDecl.Decls);
    }

    private void VisitParmVarDecl(ParmVarDecl parmVarDecl)
    {
        Debug.Assert(_outputBuilder is not null);

        if (IsExcluded(parmVarDecl))
        {
            return;
        }

        if (IsPrevContextDecl<FunctionDecl>(out var functionDecl, out _))
        {
            ForFunctionDecl(parmVarDecl, functionDecl);
        }
        else if (IsPrevContextDecl<TypedefDecl>(out var typedefDecl, out _))
        {
            ForTypedefDecl(parmVarDecl, typedefDecl);
        }
        else
        {
            _ = IsPrevContextDecl<Decl>(out var previousContext, out _);
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported parameter variable declaration parent: '{previousContext?.CursorKindSpelling}'. Generated bindings may be incomplete.", previousContext);
        }

        void ForFunctionDecl(ParmVarDecl parmVarDecl, FunctionDecl functionDecl)
        {
            var type = parmVarDecl.Type;
            var typeName = GetTargetTypeName(parmVarDecl, out var nativeTypeName);

            var name = GetRemappedCursorName(parmVarDecl);
            var escapedName = EscapeName(name);

            var functionName = GetRemappedCursorName(functionDecl);
            var isForManualImport = _config.WithManualImports.Contains(functionName);

            var parameters = functionDecl.Parameters;
            var index = parameters.IndexOf(parmVarDecl);
            var lastIndex = parameters.Count - 1;

            if (name.Equals("param", StringComparison.Ordinal))
            {
                escapedName += index;
            }

            var desc = new ParameterDesc {
                Name = escapedName,
                Type = typeName,
                NativeTypeName = nativeTypeName,
                CppAttributes = _config.GenerateCppAttributes
                    ? parmVarDecl.Attrs.Select(x => EscapeString(x.Spelling))
                    : null,
                Location = parmVarDecl.Location,
                WriteCustomAttrs = static context => {
                    (var parmVarDecl, var generator, var csharpOutputBuilder, var defaultArg) = ((ParmVarDecl, PInvokeGenerator, CSharpOutputBuilder, Expr))context;

                    generator.WithAttributes(parmVarDecl);
                    generator.WithUsings(parmVarDecl);

                    if (defaultArg is not null)
                    {
                        csharpOutputBuilder.WriteCustomAttribute("Optional, DefaultParameterValue(", () => {
                            generator.Visit(defaultArg);
                            csharpOutputBuilder.Write(')');
                        });
                    }
                    else
                    {
                        csharpOutputBuilder?.WriteCustomAttribute("Optional", null);
                    }
                },
                CustomAttrGeneratorData = (parmVarDecl, this, null as CSharpOutputBuilder, null as Expr),
                IsForManualImport = isForManualImport
            };

            var handledDefaultArg = false;
            var isExprDefaultValue = false;
            var defaultArg = (parmVarDecl.HasDefaultArg && !parmVarDecl.HasUnparsedDefaultArg) ?
                (parmVarDecl.HasUninstantiatedDefaultArg ? parmVarDecl.UninstantiatedDefaultArg : parmVarDecl.DefaultArg) :
                null;

            if (defaultArg != null)
            {
                isExprDefaultValue = IsDefaultValue(defaultArg);

                if ((_outputBuilder is CSharpOutputBuilder csharpOutputBuilder) && (_config.WithTransparentStructs.ContainsKey(typeName) || parameters.Skip(index).Any((parmVarDecl) => {
                    var type = parmVarDecl.Type;
                    var typeName = GetTargetTypeName(parmVarDecl, out var nativeTypeName);
                    return _config.WithTransparentStructs.ContainsKey(typeName);
                })))
                {
                    desc.CustomAttrGeneratorData = (parmVarDecl, this, csharpOutputBuilder, isExprDefaultValue ? null : defaultArg);
                    handledDefaultArg = true;
                }
            }

            _outputBuilder.BeginParameter(in desc);

            if (defaultArg != null && !handledDefaultArg)
            {
                _outputBuilder.BeginParameterDefault();

                if (IsTypePointerOrReference(parmVarDecl) && (defaultArg.Handle.Evaluate.Kind == CXEval_UnExposed))
                {
                    if (!isExprDefaultValue)
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"Unsupported default parameter: '{name}'. Generated bindings may be incomplete.", defaultArg);
                    }

                    var outputBuilder = StartCSharpCode();
                    outputBuilder.Write("null");
                    StopCSharpCode();
                }
                else
                {
                    Visit(defaultArg);
                }

                _outputBuilder.EndParameterDefault();
            }

            _outputBuilder.EndParameter(in desc);

            if ((index != lastIndex) || isForManualImport)
            {
                _outputBuilder.WriteParameterSeparator();
            }
        }

        void ForTypedefDecl(ParmVarDecl parmVarDecl, TypedefDecl typedefDecl)
        {
            var type = parmVarDecl.Type;
            var typeName = GetTargetTypeName(parmVarDecl, out var nativeTypeName);

            var name = GetRemappedCursorName(parmVarDecl);
            var escapedName = EscapeName(name);

            var parameters = typedefDecl.CursorChildren.OfType<ParmVarDecl>().ToList();
            var index = parameters.IndexOf(parmVarDecl);
            var lastIndex = parameters.Count - 1;

            if (name.Equals("param", StringComparison.Ordinal))
            {
                escapedName += index;
            }

            var desc = new ParameterDesc {
                Name = escapedName,
                Type = typeName,
                NativeTypeName = nativeTypeName,
                CppAttributes = _config.GenerateCppAttributes
                    ? parmVarDecl.Attrs.Select(x => EscapeString(x.Spelling))
                    : null,
                Location = parmVarDecl.Location,
                WriteCustomAttrs = static context => {
                    (var parmVarDecl, var generator) = ((ParmVarDecl, PInvokeGenerator))context;

                    generator.WithAttributes(parmVarDecl);
                    generator.WithUsings(parmVarDecl);
                },
                CustomAttrGeneratorData = (parmVarDecl, this),
            };

            _outputBuilder.BeginParameter(in desc);

            if (parmVarDecl.HasDefaultArg && !parmVarDecl.HasUnparsedDefaultArg)
            {
                _outputBuilder.BeginParameterDefault();
                Visit(parmVarDecl.HasUninstantiatedDefaultArg ? parmVarDecl.UninstantiatedDefaultArg : parmVarDecl.DefaultArg);
                _outputBuilder.EndParameterDefault();
            }

            _outputBuilder.EndParameter(in desc);

            if (index != lastIndex)
            {
                _outputBuilder.WriteParameterSeparator();
            }
        }

        bool IsDefaultValue(Expr defaultArg)
        {
            return IsStmtAsWritten<CXXNullPtrLiteralExpr>(defaultArg, out _, removeParens: true) ||
                   (IsStmtAsWritten<CastExpr>(defaultArg, out var castExpr, removeParens: true) && (castExpr.CastKind == CX_CK_NullToPointer)) ||
                   (IsStmtAsWritten<IntegerLiteral>(defaultArg, out var integerLiteral, removeParens: true) && (integerLiteral.Value == 0));
        }
    }

    private void VisitRecordDecl(RecordDecl recordDecl)
    {
        if (recordDecl.IsInjectedClassName)
        {
            // We shouldn't process injected records
            return;
        }

        var nativeName = GetCursorName(recordDecl);
        var name = GetRemappedCursorName(recordDecl);
        var escapedName = EscapeName(name);

        StartUsingOutputBuilder(name, includeTestOutput: true);
        {
            var cxxRecordDecl = recordDecl as CXXRecordDecl;
            _cxxRecordDeclContext = cxxRecordDecl;

            var hasVtbl = false;
            var hasBaseVtbl = false;

            if (cxxRecordDecl is not null)
            {
                hasVtbl = HasVtbl(cxxRecordDecl, out hasBaseVtbl);
            }

            var alignment = Math.Max(recordDecl.TypeForDecl.Handle.AlignOf, 1);
            var maxAlignm = recordDecl.Fields.Any() ? recordDecl.Fields.Max((fieldDecl) => Math.Max(fieldDecl.Type.Handle.AlignOf, 1)) : alignment;

            var isTopLevelStruct = _config.WithTypes.TryGetValue(name, out var withType) && withType.Equals("struct", StringComparison.Ordinal);
            var generateTestsClass = !recordDecl.IsAnonymous && recordDecl.DeclContext is not RecordDecl;
            var testOutputStarted = false;

            var nullableUuid = (Guid?)null;
            var uuidName = "";

            if (TryGetUuid(recordDecl, out var uuid))
            {
                nullableUuid = uuid;
                uuidName = GetRemappedName($"IID_{nativeName}", recordDecl, tryRemapOperatorName: false, out var wasRemapped, skipUsing: true);

                _uuidsToGenerate.Add(uuidName, uuid);

                if ((_testOutputBuilder is not null) && (uuid != Guid.Empty))
                {
                    StartTestOutput(ref testOutputStarted, generateTestsClass, isTopLevelStruct);

                    var className = GetClass(uuidName);

                    _testOutputBuilder.AddUsingDirective("System");

                    if (_config.DontUseUsingStaticsForGuidMember)
                    {
                        _testOutputBuilder.AddUsingDirective($"{GetNamespace(className)}");
                    }
                    else
                    {
                        _testOutputBuilder.AddUsingDirective($"static {GetNamespace(className)}.{className}");
                    }

                    _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"Guid\" /> of the <see cref=\"");
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.WriteLine("\" /> struct is correct.</summary>");

                    WithTestAttribute();

                    _testOutputBuilder.WriteIndentedLine("public static void GuidOfTest()");
                    _testOutputBuilder.WriteBlockStart();

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.WriteIndented("Assert.That");
                    }
                    else if (_config.GenerateTestsXUnit)
                    {
                        _testOutputBuilder.WriteIndented("Assert.Equal");
                    }

                    _testOutputBuilder.Write("(typeof(");
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.Write(").GUID, ");

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.Write("Is.EqualTo(");
                    }

                    var usableUuidName = uuidName;
                    if (_config.DontUseUsingStaticsForGuidMember)
                    {
                        usableUuidName = $"{className}.{usableUuidName}";
                    }

                    _testOutputBuilder.Write(usableUuidName);

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.Write(')');
                    }

                    _testOutputBuilder.Write(')');
                    _testOutputBuilder.WriteSemicolon();
                    _testOutputBuilder.WriteNewline();
                    _testOutputBuilder.WriteBlockEnd();
                    _testOutputBuilder.NeedsNewline = true;
                }
            }

            var hasGuidMember = _config.GenerateGuidMember && !string.IsNullOrWhiteSpace(uuidName);

            var layoutKind = recordDecl.IsUnion
                ? LayoutKind.Explicit
                : LayoutKind.Sequential;

            long alignment32 = -1;
            long alignment64 = -1;

            GetTypeSize(recordDecl, recordDecl.TypeForDecl, ref alignment32, ref alignment64, out var size32, out var size64);

            string[]? baseTypeNames = null;

            string? nativeNameWithExtras = null, nativeInheritance = null;
            if ((cxxRecordDecl is not null) && cxxRecordDecl.Bases.Any())
            {
                var nativeTypeNameBuilder = new StringBuilder();
                var baseTypeNamesBuilder = new List<string>();

                _ = nativeTypeNameBuilder.Append(recordDecl.IsUnion ? "union " : "struct ");
                _ = nativeTypeNameBuilder.Append(nativeName);
                _ = nativeTypeNameBuilder.Append(" : ");

                var baseName = GetRemappedCursorName(cxxRecordDecl.Bases[0].Referenced, out var nativeBaseName, skipUsing: !_config.GenerateMarkerInterfaces);

                baseTypeNamesBuilder.Add(baseName);
                _ = nativeTypeNameBuilder.Append(nativeBaseName);

                for (var i = 1; i < cxxRecordDecl.Bases.Count; i++)
                {
                    _ = nativeTypeNameBuilder.Append(", ");
                    baseName = GetRemappedCursorName(cxxRecordDecl.Bases[i].Referenced, out nativeBaseName, skipUsing: !_config.GenerateMarkerInterfaces);

                    baseTypeNamesBuilder.Add(baseName);
                    _ = nativeTypeNameBuilder.Append(nativeBaseName);
                }

                nativeNameWithExtras = nativeTypeNameBuilder.ToString();
                nativeInheritance = GetCursorName(cxxRecordDecl.Bases[cxxRecordDecl.Bases.Count - 1].Referenced);
                baseTypeNames = [.. baseTypeNamesBuilder];
            }

            if (!TryGetRemappedValue(recordDecl, _config._withPackings, out var pack))
            {
                pack = alignment < maxAlignm ? alignment.ToString(CultureInfo.InvariantCulture) : null;
            }

            var desc = new StructDesc {
                AccessSpecifier = GetAccessSpecifier(recordDecl, matchStar: true),
                EscapedName = escapedName,
                IsUnsafe = IsUnsafe(recordDecl) || hasGuidMember,
                HasVtbl = hasVtbl || hasBaseVtbl,
                IsUnion = recordDecl.IsUnion,
                Layout = new() {
                    Alignment32 = alignment32,
                    Alignment64 = alignment64,
                    Size32 = size32,
                    Size64 = size64,
                    Pack = pack,
                    MaxFieldAlignment = maxAlignm,
                    Kind = layoutKind
                },
                Uuid = nullableUuid,
                NativeType = nativeNameWithExtras,
                NativeInheritance = _config.GenerateNativeInheritanceAttribute ? nativeInheritance : null,
                Location = recordDecl.Location,
                IsNested = recordDecl.DeclContext is TagDecl,
                WriteCustomAttrs = static context => {
                    (var recordDecl, var generator) = ((RecordDecl, PInvokeGenerator))context;

                    generator.WithAttributes(recordDecl);
                    generator.WithUsings(recordDecl);
                },
                CustomAttrGeneratorData = (recordDecl, this),
            };
            Debug.Assert(_outputBuilder is not null);

            if (!isTopLevelStruct)
            {
                _outputBuilder.BeginStruct(in desc);
            }
            else
            {
                if (!_topLevelClassAttributes.TryGetValue(name, out var withAttributes))
                {
                    withAttributes = [];
                }

                if (!_topLevelClassUsings.TryGetValue(name, out var withUsings))
                {
                    withUsings = new HashSet<string>(StringComparer.Ordinal);
                }

                if (desc.LayoutAttribute is not null)
                {
                    withAttributes.Add($"StructLayout(LayoutKind.{desc.LayoutAttribute.Value}{((desc.Layout.Pack is not null) ? $", Pack = {desc.Layout.Pack}" : "")})");
                    _ = withUsings.Add("System.Runtime.InteropServices");
                }

                if (desc.Uuid.HasValue)
                {
                    withAttributes.Add($"Guid(\"{nullableUuid.GetValueOrDefault().ToString("D", CultureInfo.InvariantCulture).ToUpperInvariant()}\")");
                    _ = withUsings.Add("System.Runtime.InteropServices");
                }

                var nativeTypeName = desc.NativeType;

                if (nativeTypeName is not null)
                {
                    foreach (var entry in _config.NativeTypeNamesToStrip)
                    {
                        nativeTypeName = nativeTypeName.Replace(entry, "", StringComparison.Ordinal);
                    }

                    if (!string.IsNullOrWhiteSpace(nativeTypeName))
                    {
                        withAttributes.Add($"NativeTypeName(\"{EscapeString(nativeTypeName)}\")");
                        _ = withUsings.Add(GetNamespace("NativeTypeNameAttribute"));
                    }
                }

                if (_config.GenerateNativeInheritanceAttribute && (desc.NativeInheritance is not null))
                {
                    withAttributes.Add($"NativeInheritance(\"{desc.NativeInheritance}\")");
                    _ = withUsings.Add(GetNamespace("NativeInheritanceAttribute"));
                }

                if (_config.GenerateSourceLocationAttribute && (desc.Location is not null))
                {
                    desc.Location.Value.GetFileLocation(out var file, out var line, out var column, out _);
                    withAttributes.Add($"SourceLocation(\"{EscapeString(file.Name.ToString())}\", {line}, {column})");
                    _ = withUsings.Add(GetNamespace("SourceLocationAttribute"));
                }

                if (withAttributes.Count != 0)
                {
                    _topLevelClassAttributes[name] = withAttributes;
                }

                if (withUsings.Count != 0)
                {
                    _topLevelClassUsings[name] = withUsings;
                }

                if (desc.IsUnsafe)
                {
                    _topLevelClassIsUnsafe[name] = true;
                }

                if (hasGuidMember)
                {
                    _topLevelClassHasGuidMember[name] = true;
                }
            }

            if (hasGuidMember)
            {
                var valueDesc = new ValueDesc {
                    AccessSpecifier = AccessSpecifier.None,
                    TypeName = "Guid*",
                    EscapedName = "INativeGuid.NativeGuid",
                    ParentName = name,
                    Kind = ValueKind.GuidMember,
                    Flags = ValueFlags.Initializer,
                };

                var uuidClassName = GetClass(uuidName);

                _outputBuilder.EmitUsingDirective("System");
                _outputBuilder.EmitUsingDirective("System.Runtime.CompilerServices");

                if (_config.DontUseUsingStaticsForGuidMember)
                {
                    _outputBuilder.EmitUsingDirective($"{GetNamespace(uuidClassName)}");
                }
                else
                {
                    _outputBuilder.EmitUsingDirective($"static {GetNamespace(uuidClassName)}.{uuidClassName}");
                }

                var usableUuidName = uuidName;
                if (_config.DontUseUsingStaticsForGuidMember)
                {
                    usableUuidName = $"{uuidClassName}.{usableUuidName}";
                }

                _outputBuilder.BeginValue(in valueDesc);

                var code = _outputBuilder.BeginCSharpCode();
                code.Write("(Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in ");
                code.Write(usableUuidName);
                code.Write("))");
                _outputBuilder.EndCSharpCode(code);

                _outputBuilder.EndValue(in valueDesc);
            }

            if (hasVtbl || (hasBaseVtbl && (cxxRecordDecl is not null) && !HasBaseField(cxxRecordDecl)))
            {
                var fieldDesc = new FieldDesc {
                    AccessSpecifier = AccessSpecifier.Public,
                    NativeTypeName = null,
                    EscapedName = "lpVtbl",
                    Offset = null,
                    NeedsNewKeyword = false,
                };
                _outputBuilder.BeginField(in fieldDesc);

                if (_config.GenerateExplicitVtbls)
                {
                    if (_config.GenerateMarkerInterfaces && !_config.GenerateCompatibleCode)
                    {
                        _outputBuilder.WriteRegularField($"Vtbl<{nativeName}>*", "lpVtbl");
                    }
                    else
                    {
                        _outputBuilder.WriteRegularField("Vtbl*", "lpVtbl");
                    }
                }
                else
                {
                    _outputBuilder.WriteRegularField("void**", "lpVtbl");
                }

                _outputBuilder.EndField(in fieldDesc);
            }

            if (cxxRecordDecl is not null)
            {
                for (var index = 0; index < cxxRecordDecl.Bases.Count; index++)
                {
                    var cxxBaseSpecifier = cxxRecordDecl.Bases[index];
                    var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                    if (HasField(baseCxxRecordDecl) && !IsBaseExcluded(cxxRecordDecl, baseCxxRecordDecl, cxxBaseSpecifier, out var baseFieldName))
                    {
                        var parent = GetRemappedCursorName(baseCxxRecordDecl);

                        var fieldDesc = new FieldDesc {
                            AccessSpecifier = GetAccessSpecifier(baseCxxRecordDecl, matchStar: true),
                            NativeTypeName = null,
                            EscapedName = baseFieldName,
                            Offset = null,
                            NeedsNewKeyword = false,
                            InheritedFrom = parent,
                            Location = cxxBaseSpecifier.Location,
                        };

                        _outputBuilder.BeginField(in fieldDesc);
                        _outputBuilder.WriteRegularField(parent, baseFieldName);
                        _outputBuilder.EndField(in fieldDesc);
                    }
                }
            }

            if ((_testOutputBuilder is not null) && generateTestsClass && !_config.GenerateDisableRuntimeMarshalling)
            {
                StartTestOutput(ref testOutputStarted, generateTestsClass, isTopLevelStruct);

                _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> struct is blittable.</summary>");

                WithTestAttribute();

                _testOutputBuilder.WriteIndentedLine("public static void IsBlittableTest()");
                _testOutputBuilder.WriteBlockStart();

                WithTestAssertEqual($"sizeof({escapedName})", $"Marshal.SizeOf<{escapedName}>()");

                _testOutputBuilder.WriteBlockEnd();
                _testOutputBuilder.NeedsNewline = true;

                _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> struct has the right <see cref=\"LayoutKind\" />.</summary>");

                WithTestAttribute();

                _testOutputBuilder.WriteIndented("public static void IsLayout");

                if (recordDecl.IsUnion)
                {
                    _testOutputBuilder.Write("Explicit");
                }
                else
                {
                    _testOutputBuilder.Write("Sequential");
                }

                _testOutputBuilder.WriteLine("Test()");
                _testOutputBuilder.WriteBlockStart();

                WithTestAssertTrue($"typeof({escapedName}).Is{(recordDecl.IsUnion ? "ExplicitLayout" : "LayoutSequential")}");

                _testOutputBuilder.WriteBlockEnd();
                _testOutputBuilder.NeedsNewline = true;

                if ((size32 == 0 || size64 == 0) && !TryGetUuid(recordDecl, out _))
                {
                    AddDiagnostic(DiagnosticLevel.Info, $"{escapedName} has a size of 0", recordDecl);
                }

                _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> struct has the correct size.</summary>");

                WithTestAttribute();

                _testOutputBuilder.WriteIndentedLine("public static void SizeOfTest()");
                _testOutputBuilder.WriteBlockStart();

                if (size32 != size64)
                {
                    _testOutputBuilder.AddUsingDirective("System");

                    _testOutputBuilder.WriteIndentedLine("if (Environment.Is64BitProcess)");
                    _testOutputBuilder.WriteBlockStart();

                    WithTestAssertEqual($"{Math.Max(size64, 1)}", $"sizeof({escapedName})");

                    _testOutputBuilder.WriteBlockEnd();
                    _testOutputBuilder.WriteIndentedLine("else");
                    _testOutputBuilder.WriteBlockStart();
                }

                WithTestAssertEqual($"{Math.Max(size32, 1)}", $"sizeof({escapedName})");

                if (size32 != size64)
                {
                    _testOutputBuilder.WriteBlockEnd();
                }

                _testOutputBuilder.WriteBlockEnd();
            }

            var bitfieldDescs = GetBitfieldDescs(recordDecl);
            var bitfieldIndex = (bitfieldDescs.Length == 1) ? -1 : 0;

            var bitfieldPreviousSize = 0L;
            var bitfieldRemainingBits = 0L;

            foreach (var fieldDecl in recordDecl.Fields)
            {
                if (fieldDecl.IsBitField)
                {
                    VisitBitfieldDecl(fieldDecl, bitfieldDescs, recordDecl, contextName: "", ref bitfieldIndex, ref bitfieldPreviousSize, ref bitfieldRemainingBits);
                }
                else
                {
                    bitfieldPreviousSize = 0;
                    bitfieldRemainingBits = 0;
                }

                Visit(fieldDecl);
                _outputBuilder.WriteDivider();
            }

            Visit(recordDecl.IndirectFields);

            if (cxxRecordDecl is not null)
            {
                foreach (var cxxConstructorDecl in cxxRecordDecl.Ctors)
                {
                    if (!IsExcluded(cxxConstructorDecl))
                    {
                        Visit(cxxConstructorDecl);
                        _outputBuilder.WriteDivider();
                    }
                }

                if (cxxRecordDecl.HasUserDeclaredDestructor)
                {
                    var cxxDestructorDecl = cxxRecordDecl.Destructor;

                    if (cxxDestructorDecl == null)
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, "Record has user declared destructor, but Destructor property was null. Generated bindings may be incomplete.", cxxRecordDecl);
                    }
                    else if (!cxxDestructorDecl.IsVirtual && !IsExcluded(cxxDestructorDecl))
                    {
                        Visit(cxxDestructorDecl);
                        _outputBuilder.WriteDivider();
                    }
                }

                if (hasVtbl || hasBaseVtbl)
                {
                    OutputDelegateSignatures(cxxRecordDecl, cxxRecordDecl);
                }
            }

            var excludedCursors = recordDecl.Fields.AsEnumerable<Cursor>().Concat(recordDecl.IndirectFields);

            if (cxxRecordDecl is not null)
            {
                OutputMethods(cxxRecordDecl, cxxRecordDecl);
                excludedCursors = excludedCursors.Concat(cxxRecordDecl.Methods);
            }

            Visit(recordDecl.Decls, excludedCursors);

            foreach (var array in recordDecl.Fields.Where(IsTypeConstantOrIncompleteArray))
            {
                VisitConstantOrIncompleteArrayFieldDecl(recordDecl, array);
            }

            if (hasVtbl || hasBaseVtbl)
            {
                Debug.Assert(cxxRecordDecl is not null);

                if (!_config.GenerateCompatibleCode)
                {
                    _outputBuilder.EmitCompatibleCodeSupport();
                }

                if (_config.ExcludeFnptrCodegen)
                {
                    _outputBuilder.EmitFnPtrSupport();
                }

                OutputVtblHelperMethods(cxxRecordDecl, cxxRecordDecl);

                if (_config.GenerateMarkerInterfaces)
                {
                    if (_outputBuilder is CSharpOutputBuilder csharpOutputBuilder)
                    {
                        csharpOutputBuilder.NeedsNewline = true;
                    }

                    _outputBuilder.BeginMarkerInterface(baseTypeNames);
                    OutputMarkerInterfaces(cxxRecordDecl, cxxRecordDecl);
                    _outputBuilder.EndMarkerInterface();
                }

                if (_config.GenerateExplicitVtbls || _config.GenerateTrimmableVtbls)
                {
                    if (_outputBuilder is CSharpOutputBuilder csharpOutputBuilder)
                    {
                        csharpOutputBuilder.NeedsNewline = true;
                    }

                    _outputBuilder.BeginExplicitVtbl();
                    OutputVtblEntries(cxxRecordDecl, cxxRecordDecl);
                    _outputBuilder.EndExplicitVtbl();
                }
            }

            if (!isTopLevelStruct)
            {
                _outputBuilder.EndStruct(in desc);

                if ((_testOutputBuilder is not null) && generateTestsClass && testOutputStarted)
                {
                    _testOutputBuilder.WriteBlockEnd();
                }
            }

            _cxxRecordDeclContext = null;
        }
        StopUsingOutputBuilder();

        string FixupNameForMultipleHits(CXXMethodDecl cxxMethodDecl)
        {
            var remappedName = GetRemappedCursorName(cxxMethodDecl);
            var overloadIndex = GetOverloadIndex(cxxMethodDecl);

            if (overloadIndex != 0)
            {
                remappedName = $"{remappedName}{overloadIndex}";
            }
            return remappedName;
        }

        void OutputDelegateSignatures(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
        {
            if (!_config.ExcludeFnptrCodegen)
            {
                return;
            }

            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                OutputDelegateSignatures(rootCxxRecordDecl, baseCxxRecordDecl);
            }

            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    if (!cxxMethodDecl.IsVirtual)
                    {
                        continue;
                    }

                    if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
                    {
                        continue;
                    }

                    _outputBuilder.WriteDivider();

                    var remappedName = FixupNameForMultipleHits(cxxMethodDecl);
                    Debug.Assert(CurrentContext.Cursor == rootCxxRecordDecl);
                    Visit(cxxMethodDecl);
                }
            }
        }

        void OutputMethods(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
        {
            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                OutputMethods(rootCxxRecordDecl, baseCxxRecordDecl);
            }

            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    if (cxxMethodDecl.IsVirtual)
                    {
                        continue;
                    }

                    if (cxxMethodDecl is CXXConstructorDecl or CXXDestructorDecl)
                    {
                        continue;
                    }

                    Debug.Assert(CurrentContext.Cursor == rootCxxRecordDecl);
                    Visit(cxxMethodDecl);
                    _outputBuilder.WriteDivider();
                }
            }
        }

        void OutputMarkerInterface(CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl)
        {
            if (!cxxMethodDecl.IsVirtual)
            {
                return;
            }

            if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
            {
                return;
            }

            if (_config.GenerateTrimmableVtbls && cxxMethodDecl.Parameters.Any((parmVarDecl) => IsType<PointerType>(parmVarDecl, parmVarDecl.Type, out var pointerType) &&
                                                                                                IsType<FunctionType>(parmVarDecl, pointerType.PointeeType, out _)))
            {
                // This breaks trimming right now
                return;
            }

            var currentContext = _context.AddLast((cxxMethodDecl, null));

            var returnType = cxxMethodDecl.ReturnType;
            var returnTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, returnType, out var nativeTypeName);

            var remappedName = FixupNameForMultipleHits(cxxMethodDecl);
            var name = GetRemappedCursorName(cxxMethodDecl);
            var needsReturnFixup = false;
            var needsCastToTransparentStruct = false;

            if (!IsTypeVoid(cxxMethodDecl, returnType))
            {
                needsReturnFixup = NeedsReturnFixup(cxxMethodDecl);
                needsCastToTransparentStruct = _config.WithTransparentStructs.TryGetValue(returnTypeName, out var transparentStruct) && IsTransparentStructHandle(transparentStruct.Kind);
            }

            var desc = new FunctionOrDelegateDesc {
                AccessSpecifier = AccessSpecifier.Public,
                EscapedName = EscapeAndStripMethodName(name),
                IsMemberFunction = true,
                NativeTypeName = nativeTypeName,
                HasFnPtrCodeGen = !_config.ExcludeFnptrCodegen,
                IsCtxCxxRecord = true,
                IsCxxRecordCtxUnsafe = IsUnsafe(cxxRecordDecl),
                IsUnsafe = true,
                NeedsReturnFixup = needsReturnFixup,
                ReturnType = returnTypeName,
                VtblIndex = _config.GenerateVtblIndexAttribute ? cxxMethodDecl.VtblIndex : -1,
                Location = cxxMethodDecl.Location,
                WriteCustomAttrs = static context => {
                    (var cxxMethodDecl, var generator) = ((CXXMethodDecl, PInvokeGenerator))context;

                    generator.WithAttributes(cxxMethodDecl);
                    generator.WithUsings(cxxMethodDecl);
                },
                CustomAttrGeneratorData = (cxxMethodDecl, this),
            };

            var isUnsafe = true;
            _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);

            _outputBuilder.BeginFunctionInnerPrototype(in desc);

            Visit(cxxMethodDecl.Parameters);

            _outputBuilder.EndFunctionInnerPrototype(in desc);
            _outputBuilder.EndFunctionOrDelegate(in desc);

            Debug.Assert(_context.Last == currentContext);
            _context.RemoveLast();
        }

        void OutputMarkerInterfaces(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
        {
            // Don't process the base types because that's exposed via inheritance instead
            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    OutputMarkerInterface(rootCxxRecordDecl, cxxMethodDecl);
                }
            }
        }

        void OutputVtblEntries(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
        {
            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                OutputVtblEntries(rootCxxRecordDecl, baseCxxRecordDecl);
            }

            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    OutputVtblEntry(rootCxxRecordDecl, cxxMethodDecl);
                }
            }
        }

        void OutputVtblEntry(CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl)
        {
            if (!cxxMethodDecl.IsVirtual)
            {
                return;
            }

            if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
            {
                return;
            }

            var cxxMethodDeclTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, cxxMethodDecl.Type, out var nativeTypeName, skipUsing: false, ignoreTransparentStructsWhereRequired: true);

            if (_config.GenerateMarkerInterfaces && !_config.ExcludeFnptrCodegen)
            {
                var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
                cxxMethodDeclTypeName = cxxMethodDeclTypeName.Replace($"<{cxxRecordDeclName}*,", "<TSelf*,", StringComparison.Ordinal);
            }

            var remappedName = FixupNameForMultipleHits(cxxMethodDecl);
            var escapedName = EscapeAndStripMethodName(remappedName);

            var desc = new FieldDesc {
                AccessSpecifier = AccessSpecifier.Public,
                NativeTypeName = nativeTypeName,
                EscapedName = escapedName,
                Offset = null,
                NeedsNewKeyword = NeedsNewKeyword(remappedName),
                Location = cxxMethodDecl.Location,
                WriteCustomAttrs = static context => {
                    (var cxxMethodDecl, var generator) = ((CXXMethodDecl, PInvokeGenerator))context;

                    generator.WithAttributes(cxxMethodDecl);
                    generator.WithUsings(cxxMethodDecl);
                },
                CustomAttrGeneratorData = (cxxMethodDecl, this),
            };

            _outputBuilder.BeginField(in desc);
            _outputBuilder.WriteRegularField(cxxMethodDeclTypeName, escapedName);
            _outputBuilder.EndField(in desc);

            _outputBuilder.WriteDivider();
        }

        void OutputVtblHelperMethod(CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl)
        {
            if (!cxxMethodDecl.IsVirtual)
            {
                return;
            }

            if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
            {
                return;
            }

            var currentContext = _context.AddLast((cxxMethodDecl, null));

            var returnType = cxxMethodDecl.ReturnType;
            var returnTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, returnType, out var nativeTypeName);

            var remappedName = FixupNameForMultipleHits(cxxMethodDecl);
            var name = GetRemappedCursorName(cxxMethodDecl);
            var needsReturnFixup = false;
            var needsCastToTransparentStruct = false;
            var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
            var parentName = cxxRecordDeclName;
            var isInherited = false;

            var parent = cxxMethodDecl.Parent;
            Debug.Assert(parent is not null);

            if (parent != cxxRecordDecl)
            {
                parentName = GetRemappedCursorName(parent);
                isInherited = true;
            }

            if (!IsTypeVoid(cxxMethodDecl, returnType))
            {
                needsReturnFixup = NeedsReturnFixup(cxxMethodDecl);
                needsCastToTransparentStruct = _config.WithTransparentStructs.TryGetValue(returnTypeName, out var transparentStruct) && IsTransparentStructHandle(transparentStruct.Kind);
            }

            var desc = new FunctionOrDelegateDesc {
                AccessSpecifier = AccessSpecifier.Public,
                IsAggressivelyInlined = _config.GenerateAggressiveInlining,
                EscapedName = EscapeAndStripMethodName(name),
                ParentName = parentName,
                IsMemberFunction = true,
                IsInherited = isInherited,
                NativeTypeName = nativeTypeName,
                NeedsNewKeyword = NeedsNewKeyword(name, cxxMethodDecl.Parameters),
                HasFnPtrCodeGen = !_config.ExcludeFnptrCodegen,
                IsCtxCxxRecord = true,
                IsCxxRecordCtxUnsafe = IsUnsafe(cxxRecordDecl),
                IsUnsafe = true,
                NeedsReturnFixup = needsReturnFixup,
                ReturnType = returnTypeName,
                VtblIndex = _config.GenerateVtblIndexAttribute ? cxxMethodDecl.VtblIndex : -1,
                Location = cxxMethodDecl.Location,
                HasBody = true,
                WriteCustomAttrs = static context => {
                    (var cxxMethodDecl, var generator) = ((CXXMethodDecl, PInvokeGenerator))context;

                    generator.WithAttributes(cxxMethodDecl);
                    generator.WithUsings(cxxMethodDecl);
                },
                CustomAttrGeneratorData = (cxxMethodDecl, this),
            };

            var isUnsafe = true;
            _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);

            _outputBuilder.BeginFunctionInnerPrototype(in desc);

            Visit(cxxMethodDecl.Parameters);

            _outputBuilder.EndFunctionInnerPrototype(in desc);
            _outputBuilder.BeginBody();

            var escapedCXXRecordDeclName = EscapeName(cxxRecordDeclName);

            _outputBuilder.BeginInnerFunctionBody();
            var body = _outputBuilder.BeginCSharpCode();

            if (_config.GenerateCompatibleCode)
            {
                body.Write("fixed (");
                body.Write(escapedCXXRecordDeclName);
                body.WriteLine("* pThis = &this)");
                body.WriteBlockStart();
                body.WriteIndentation();
            }

            if (needsReturnFixup)
            {
                body.BeginMarker("fixup", new KeyValuePair<string, object>("type", "*result"));
                body.Write(returnTypeName);
                body.EndMarker("fixup");
                body.Write(" result");
                body.WriteSemicolon();
                body.WriteNewline();
                body.WriteIndentation();
            }

            if (!IsTypeVoid(cxxMethodDecl, returnType))
            {
                body.Write("return ");
            }

            if (needsCastToTransparentStruct)
            {
                body.Write("((");
                body.Write(returnTypeName);
                body.Write(")(");
            }

            if (needsReturnFixup)
            {
                body.Write('*');
            }

            if (_config.ExcludeFnptrCodegen)
            {
                body.Write("Marshal.GetDelegateForFunctionPointer<");
                body.BeginMarker("delegate");
                body.Write(PrefixAndStripMethodName(name, GetOverloadIndex(cxxMethodDecl)));
                body.EndMarker("delegate");
                body.Write(">(");
            }

            if (_config.GenerateExplicitVtbls)
            {
                body.Write("lpVtbl->");
                body.BeginMarker("vtbl", new KeyValuePair<string, object>("explicit", true));
                body.Write(EscapeAndStripMethodName(remappedName));
                body.EndMarker("vtbl");
            }
            else
            {
                var cxxMethodDeclTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, cxxMethodDecl.Type, out _, skipUsing: false, ignoreTransparentStructsWhereRequired: true);

                if (!_config.ExcludeFnptrCodegen)
                {
                    body.Write('(');
                }

                body.Write('(');
                body.Write(cxxMethodDeclTypeName);
                body.Write(")(lpVtbl[");
                body.BeginMarker("vtbl", new KeyValuePair<string, object>("explicit", false));
                body.Write(cxxMethodDecl.VtblIndex);
                body.EndMarker("vtbl");
                body.Write("])");

                if (!_config.ExcludeFnptrCodegen)
                {
                    body.Write(')');
                }
            }

            if (_config.ExcludeFnptrCodegen)
            {
                body.Write(')');
            }

            body.Write('(');

            body.BeginMarker("param", new KeyValuePair<string, object>("special", "thisPtr"));
            if (_config.GenerateCompatibleCode)
            {
                body.Write("pThis");
            }
            else
            {
                body.Write('(');
                body.Write(escapedCXXRecordDeclName);
                body.Write("*)Unsafe.AsPointer(ref this)");
            }
            body.EndMarker("param");

            if (needsReturnFixup)
            {
                body.BeginMarker("param", new KeyValuePair<string, object>("special", "retFixup"));
                body.Write(", &result");
                body.EndMarker("param");
            }

            var parmVarDecls = cxxMethodDecl.Parameters;

            for (var index = 0; index < parmVarDecls.Count; index++)
            {
                body.Write(", ");

                var parmVarDeclName = GetRemappedCursorName(parmVarDecls[index]);
                var escapedParmVarDeclName = EscapeName(parmVarDeclName);

                if (parmVarDeclName.Equals("param", StringComparison.Ordinal))
                {
                    escapedParmVarDeclName += index;
                }

                body.BeginMarker("param", new KeyValuePair<string, object>("name", escapedParmVarDeclName));
                body.Write(escapedParmVarDeclName);
                body.EndMarker("param");
            }

            body.Write(')');

            if (!_config.GenerateDisableRuntimeMarshalling && returnTypeName.Equals("bool", StringComparison.Ordinal))
            {
                // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                body.Write(" != 0");
            }

            if (needsCastToTransparentStruct)
            {
                body.Write("))");
            }

            body.WriteSemicolon();
            body.WriteNewline();

            if (_config.GenerateCompatibleCode)
            {
                body.WriteBlockEnd();
            }

            _outputBuilder.EndCSharpCode(body);
            _outputBuilder.EndInnerFunctionBody();
            _outputBuilder.EndBody();
            _outputBuilder.EndFunctionOrDelegate(in desc);

            Debug.Assert(_context.Last == currentContext);
            _context.RemoveLast();
        }

        void OutputVtblHelperMethods(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
        {
            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                OutputVtblHelperMethods(rootCxxRecordDecl, baseCxxRecordDecl);
            }

            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    _outputBuilder.WriteDivider();
                    OutputVtblHelperMethod(rootCxxRecordDecl, cxxMethodDecl);
                }
            }
        }

        void VisitBitfieldDecl(FieldDecl fieldDecl, BitfieldDesc[] bitfieldDescs, RecordDecl recordDecl, string contextName, ref int index, ref long previousSize, ref long remainingBits)
        {
            Debug.Assert(fieldDecl.IsBitField);

            var typeName = GetRemappedTypeName(fieldDecl, context: null, fieldDecl.Type, out var nativeTypeName);

            if (string.IsNullOrWhiteSpace(nativeTypeName))
            {
                nativeTypeName = typeName;
            }

            nativeTypeName += $" : {fieldDecl.BitWidthValue}";

            var currentSize = fieldDecl.Type.Handle.SizeOf;
            var bitfieldName = "_bitfield";

            Type typeBacking;
            string typeNameBacking;

            var parent = fieldDecl.Parent;
            Debug.Assert(parent is not null);

            if ((!_config.GenerateUnixTypes && (currentSize != previousSize)) || (fieldDecl.BitWidthValue > remainingBits))
            {
                if (index >= 0)
                {
                    index++;
                    bitfieldName += index.ToString(CultureInfo.InvariantCulture);
                }

                remainingBits = currentSize * 8;
                previousSize = 0;

                var bitfieldDesc = (index > 0) ? bitfieldDescs[index - 1] : bitfieldDescs[0];
                typeBacking = bitfieldDesc.TypeBacking;
                typeNameBacking = GetRemappedTypeName(fieldDecl, context: null, typeBacking, out _);

                if (parent == recordDecl)
                {
                    var fieldDesc = new FieldDesc {
                        AccessSpecifier = AccessSpecifier.Public,
                        NativeTypeName = null,
                        EscapedName = bitfieldName,
                        Offset = parent.IsUnion ? 0 : null,
                        NeedsNewKeyword = false,
                        Location = fieldDecl.Location,
                        WriteCustomAttrs = static context => {
                            (var bitfieldDesc, var generator) = ((BitfieldDesc, PInvokeGenerator))context;

                            if (!generator.Config.GenerateNativeBitfieldAttribute)
                            {
                                return;
                            }

                            var outputBuilder = generator._outputBuilder;
                            Debug.Assert(outputBuilder is not null);

                            foreach (var bitfieldRegion in bitfieldDesc.Regions)
                            {
                                outputBuilder.WriteCustomAttribute($"NativeBitfield(\"{bitfieldRegion.Name}\", offset: {bitfieldRegion.Offset}, length: {bitfieldRegion.Length})");
                            }

                            var namespaceName = generator.GetNamespace("NativeBitfieldAttribute");
                            generator.AddUsingDirective(outputBuilder, namespaceName);
                        },
                        CustomAttrGeneratorData = (bitfieldDesc, this),
                    };
                    _outputBuilder.BeginField(in fieldDesc);
                    _outputBuilder.WriteRegularField(typeNameBacking, bitfieldName);
                    _outputBuilder.EndField(in fieldDesc);
                }
            }
            else
            {
                currentSize = Math.Max(previousSize, currentSize);

                if (_config.GenerateUnixTypes && (currentSize > previousSize))
                {
                    remainingBits += (currentSize - previousSize) * 8;
                }

                if (index >= 0)
                {
                    bitfieldName += index.ToString(CultureInfo.InvariantCulture);
                }

                var bitfieldDesc = (index > 0) ? bitfieldDescs[index - 1] : bitfieldDescs[0];
                typeBacking = bitfieldDesc.TypeBacking;
                typeNameBacking = GetRemappedTypeName(fieldDecl, context: null, typeBacking, out _);
            }

            var bitfieldOffset = (currentSize * 8) - remainingBits;
            var bitwidthHexStringBacking = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X", CultureInfo.InvariantCulture);

            if (!IsType<BuiltinType>(fieldDecl, typeBacking, out var builtinTypeBacking))
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{typeBacking.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                return;
            }

            var isTypeBackingSigned = false;

            switch (builtinTypeBacking.Kind)
            {
                case CXType_Char_U:
                case CXType_UChar:
                case CXType_UShort:
                case CXType_UInt:
                {
                    bitwidthHexStringBacking += "u";
                    break;
                }

                case CXType_ULong:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }

                    goto case CXType_UInt;
                }

                case CXType_ULongLong:
                {
                    if (typeNameBacking.Equals("nuint", StringComparison.Ordinal))
                    {
                        goto case CXType_UInt;
                    }

                    bitwidthHexStringBacking += "UL";
                    break;
                }

                case CXType_Char_S:
                case CXType_SChar:
                case CXType_Short:
                case CXType_Int:
                {
                    isTypeBackingSigned = true;
                    break;
                }

                case CXType_Long:
                {
                    isTypeBackingSigned = true;

                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }
                    break;
                }

                case CXType_LongLong:
                {
                    isTypeBackingSigned = true;

                    if (typeNameBacking.Equals("nint", StringComparison.Ordinal))
                    {
                        goto case CXType_Int;
                    }

                    bitwidthHexStringBacking += "L";
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{builtinTypeBacking.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                    break;
                }
            }

            var bitwidthHexString = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X", CultureInfo.InvariantCulture);
            var type = fieldDecl.Type;

            if (IsType<BuiltinType>(fieldDecl, type, out var builtinType))
            {
                type = builtinType;
            }
            else if (IsType<EnumType>(fieldDecl, type, out var enumType))
            {
                if (IsType<BuiltinType>(fieldDecl, enumType.Decl.IntegerType, out builtinType))
                {
                    type = enumType;
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{enumType.Decl.IntegerType.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                    return;
                }
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{fieldDecl.Type.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                return;
            }

            var isTypeSigned = false;

            switch (builtinType.Kind)
            {
                case CXType_Char_U:
                case CXType_UChar:
                case CXType_UShort:
                case CXType_UInt:
                {
                    bitwidthHexString += "u";
                    break;
                }

                case CXType_ULong:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }

                    goto case CXType_UInt;
                }

                case CXType_ULongLong:
                {
                    if (typeNameBacking.Equals("nuint", StringComparison.Ordinal))
                    {
                        goto case CXType_UInt;
                    }

                    bitwidthHexString += "UL";
                    break;
                }

                case CXType_Char_S:
                case CXType_SChar:
                case CXType_Short:
                case CXType_Int:
                {
                    isTypeSigned = true;
                    break;
                }

                case CXType_Long:
                {
                    isTypeSigned = true;

                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }
                    break;
                }

                case CXType_LongLong:
                {
                    isTypeSigned = true;

                    if (typeNameBacking.Equals("nint", StringComparison.Ordinal))
                    {
                        goto case CXType_Int;
                    }

                    bitwidthHexString += "L";
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{builtinType.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                    break;
                }
            }

            var accessSpecifier = GetAccessSpecifier(fieldDecl, matchStar: false);
            var name = GetRemappedCursorName(fieldDecl);
            var escapedName = EscapeName(name);

            var desc = new FieldDesc {
                AccessSpecifier = accessSpecifier,
                NativeTypeName = nativeTypeName,
                EscapedName = escapedName,
                ParentName = GetRemappedCursorName(parent),
                Offset = null,
                NeedsNewKeyword = false,
                Location = fieldDecl.Location,
                HasBody = true,
                WriteCustomAttrs = static context => {
                    (var fieldDecl, var generator) = ((FieldDecl, PInvokeGenerator))context;

                    generator.WithAttributes(fieldDecl);
                    generator.WithUsings(fieldDecl);
                },
                CustomAttrGeneratorData = (fieldDecl, this),
            };

            _outputBuilder.WriteDivider();
            _outputBuilder.BeginField(in desc);
            _outputBuilder.WriteRegularField(typeName, escapedName);
            _outputBuilder.BeginBody();

            var recordDeclName = GetCursorName(recordDecl);

            // Small types become uint32/int32s after shifting
            var isSmallType = fieldDecl.Type.Handle.SizeOf < 4;
            var isSmallTypeBacking = currentSize < 4;

            // Check if input/output types mismatch
            var isTypeMismatch = type != typeBacking;

            // Signed types are sign extended when shifted
            var isUnsignedToSigned = !isTypeBackingSigned && isTypeSigned;
            
            // Check if type is directly shiftable/maskable
            // Remapped types are not guaranteed to be shiftable or maskable
            // Enums are maskable, but not shiftable
            var isTypeLikelyRemapped = type == typeBacking && typeName != typeNameBacking;
            var isTypeAnEnum = IsType<EnumType>(fieldDecl);

            // Main cases:
            // backing  int, current int            (value << cns) >> cns
            // backing  int, current uint           (uint)((value >> cns) & msk)

            // backing uint, current int            (int)(value << cns) >> cns
            // backing uint, current uint           (value >> cns) & msk

            // backing uint, current byte           (byte)((value >> cns) & msk)
            // backing uint, current sbyte          (sbyte)((sbyte)(value << cns) >> cns)

            // Getter
            {
                _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining,
                    isReadOnly: !Config.GenerateCompatibleCode);
                var code = _outputBuilder.BeginCSharpCode();

                code.WriteIndented("return ");

                var needsCastToFinal = (isSmallType || isTypeMismatch || isUnsignedToSigned) && !isTypeAnEnum;

                // This is to handle the "backing uint, current sbyte" case
                var needsCastToFinalAgain = isUnsignedToSigned && isSmallType;
                if (needsCastToFinalAgain)
                {
                    code.Write('(');
                    code.BeginMarker("typeName");
                    code.Write(typeName);
                    code.EndMarker("typeName");
                    code.Write(")(");
                }

                if (needsCastToFinal)
                {
                    code.Write('(');
                    code.BeginMarker("typeName");
                    code.Write(typeName);
                    code.EndMarker("typeName");
                    code.Write(")");
                }

                var needsCastToFinalParen = needsCastToFinal && !isUnsignedToSigned;
                if (needsCastToFinalParen)
                {
                    code.Write('(');
                }

                var needsCastBeforeOp = isTypeLikelyRemapped || isTypeAnEnum;
                if (needsCastBeforeOp)
                {
                    code.Write('(');
                    code.BeginMarker("typeName");
                    code.Write(typeName);
                    code.EndMarker("typeName");
                    code.Write(")(");
                }

                if (isTypeSigned)
                {
                    code.Write('(');

                    if (!string.IsNullOrWhiteSpace(contextName))
                    {
                        code.BeginMarker("contextName");
                        code.Write(contextName);
                        code.EndMarker("contextName");
                        code.Write('.');
                    }

                    code.BeginMarker("bitfieldName");
                    code.Write(bitfieldName);
                    code.EndMarker("bitfieldName");

                    code.Write(" << ");
                    code.BeginMarker("remainingBitsMinusBitWidth");
                    code.Write(remainingBits - fieldDecl.BitWidthValue);
                    code.EndMarker("remainingBitsMinusBitWidth");

                    code.Write(')');

                    code.Write(" >> ");
                    code.BeginMarker("currentSizeMinusBitWidth");
                    code.Write((currentSize * 8) - fieldDecl.BitWidthValue);
                    code.EndMarker("currentSizeMinusBitWidth");
                }
                else
                {
                    var needsOffset = bitfieldOffset != 0;
                    if (needsOffset)
                    {
                        code.Write('(');
                    }

                    if (!string.IsNullOrWhiteSpace(contextName))
                    {
                        code.BeginMarker("contextName");
                        code.Write(contextName);
                        code.EndMarker("contextName");
                        code.Write('.');
                    }

                    code.BeginMarker("bitfieldName");
                    code.Write(bitfieldName);
                    code.EndMarker("bitfieldName");

                    if (needsOffset)
                    {
                        code.Write(" >> ");
                        code.BeginMarker("bitfieldOffset");
                        code.Write(bitfieldOffset);
                        code.EndMarker("bitfieldOffset");

                        code.Write(')');
                    }

                    code.Write(" & 0x");
                    code.BeginMarker("bitwidthHexStringBacking");
                    code.Write(bitwidthHexStringBacking);
                    code.EndMarker("bitwidthHexStringBacking");
                }

                if (needsCastBeforeOp)
                {
                    code.Write(')');
                }

                if (needsCastToFinalParen)
                {
                    code.Write(')');
                }

                if (needsCastToFinalAgain)
                {
                    code.Write(')');
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);
                _outputBuilder.EndGetter();
            }

            // Setter
            {
                _outputBuilder.BeginSetter(_config.GenerateAggressiveInlining);
                var code = _outputBuilder.BeginCSharpCode();
                code.WriteIndentation();

                if (!string.IsNullOrWhiteSpace(contextName))
                {
                    code.BeginMarker("contextName");
                    code.Write(contextName);
                    code.EndMarker("contextName");
                    code.Write('.');
                }

                code.BeginMarker("bitfieldName");
                code.Write(bitfieldName);
                code.EndMarker("bitfieldName");

                code.Write(" = ");

                var needsCastToFinal = isSmallTypeBacking;
                if (needsCastToFinal)
                {
                    code.Write('(');
                    code.BeginMarker("typeNameBacking");
                    code.Write(typeNameBacking);
                    code.EndMarker("typeNameBacking");
                    code.Write(")(");
                }

                // Zero out target bits
                code.Write('(');

                if (!string.IsNullOrWhiteSpace(contextName))
                {
                    code.Write(contextName);
                    code.Write('.');
                }

                code.Write(bitfieldName);

                code.Write(" & ~");

                if (bitfieldOffset != 0)
                {
                    code.Write('(');
                }

                code.Write("0x");
                code.BeginMarker("bitwidthHexStringBacking");
                code.Write(bitwidthHexStringBacking);
                code.EndMarker("bitwidthHexStringBacking");

                if (bitfieldOffset != 0)
                {
                    code.Write(" << ");
                    code.BeginMarker("bitfieldOffset");
                    code.Write(bitfieldOffset);
                    code.EndMarker("bitfieldOffset");
                    code.Write(')');
                }

                // Write to target bits
                code.Write(") | ");

                var needsCastBeforeLogicalOr = isTypeMismatch && !isTypeAnEnum;
                if (needsCastBeforeLogicalOr)
                {
                    code.Write('(');
                    code.Write(typeNameBacking);
                    code.Write(")");
                }

                code.Write('(');

                if (bitfieldOffset != 0)
                {
                    code.Write('(');
                }

                var needsCastBeforeOp = isTypeLikelyRemapped || isTypeAnEnum;
                if (needsCastBeforeOp)
                {
                    code.Write('(');
                    code.Write(typeNameBacking);
                    code.Write(")(value)");
                }
                else
                {
                    code.Write("value");
                }

                code.Write(" & 0x");
                code.BeginMarker("bitwidthHexString");
                code.Write(bitwidthHexString);
                code.EndMarker("bitwidthHexString");

                if (bitfieldOffset != 0)
                {
                    code.Write(") << ");
                    code.Write(bitfieldOffset);
                }

                code.Write(')');

                if (needsCastToFinal)
                {
                    code.Write(')');
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);
                _outputBuilder.EndSetter();
            }

            _outputBuilder.EndBody();
            _outputBuilder.EndField(in desc);
            _outputBuilder.WriteDivider();

            remainingBits -= fieldDecl.BitWidthValue;
            previousSize = Math.Max(previousSize, currentSize);
        }

        void VisitConstantOrIncompleteArrayFieldDecl(RecordDecl recordDecl, FieldDecl constantOrIncompleteArray)
        {
            if (!IsTypeConstantOrIncompleteArray(constantOrIncompleteArray, out var arrayType))
            {
                AddDiagnostic(DiagnosticLevel.Error, "Expected constant or incomplete array. Generated bindings may be incomplete", constantOrIncompleteArray);
                return;
            }

            var outputBuilder = _outputBuilder;
            var arrayTypeName = GetRemappedTypeName(constantOrIncompleteArray, context: null, arrayType, out _);

            if (IsSupportedFixedSizedBufferType(arrayTypeName))
            {
                return;
            }

            _outputBuilder.WriteDivider();

            var alignment = Math.Max(recordDecl.TypeForDecl.Handle.AlignOf, 1);
            var maxAlignm = recordDecl.Fields.Any() ? recordDecl.Fields.Max((fieldDecl) => Math.Max(fieldDecl.Type.Handle.AlignOf, 1)) : alignment;

            var accessSpecifier = GetAccessSpecifier(constantOrIncompleteArray, matchStar: false);
            var elementType = arrayType.ElementType;
            var isUnsafeElementType = IsTypePointerOrReference(constantOrIncompleteArray, arrayType.ElementType) &&
                                      !arrayTypeName.Equals("IntPtr", StringComparison.Ordinal) && !arrayTypeName.Equals("UIntPtr", StringComparison.Ordinal);

            var name = GetArtificialFixedSizedBufferName(constantOrIncompleteArray);
            var escapedName = EscapeName(name);

            var arraySize = Math.Max((arrayType as ConstantArrayType)?.Size ?? 0, 1);
            var totalSize = arraySize;
            var totalSizeString = $"{arraySize}";
            var sizePerDimension = new List<(long index, long size)>() { (0, arraySize) };

            while (IsTypeConstantOrIncompleteArray(recordDecl, elementType, out var subArrayType))
            {
                var subArraySize = Math.Max((subArrayType as ConstantArrayType)?.Size ?? 0, 1);
                totalSize *= subArraySize;
                totalSizeString += $" * {subArraySize}";
                sizePerDimension.Add((0, subArraySize));

                elementType = subArrayType.ElementType;
            }

            long alignment32 = -1;
            long alignment64 = -1;

            GetTypeSize(constantOrIncompleteArray, constantOrIncompleteArray.Type, ref alignment32, ref alignment64, out var size32, out var size64);

            if ((size32 == 0 || size64 == 0) && _testOutputBuilder != null)
            {
                AddDiagnostic(DiagnosticLevel.Info, $"{escapedName} (constant array field) has a size of 0", constantOrIncompleteArray);
            }

            if (!_config.GenerateLatestCode || (totalSize <= 1) || isUnsafeElementType)
            {
                totalSizeString = null;
            }

            var desc = new StructDesc {
                AccessSpecifier = accessSpecifier,
                EscapedName = escapedName,
                IsUnsafe = isUnsafeElementType,
                Layout = new() {
                    Alignment32 = alignment32,
                    Alignment64 = alignment64,
                    Size32 = size32,
                    Size64 = size64,
                    Pack = alignment < maxAlignm ? alignment.ToString(CultureInfo.InvariantCulture) : null,
                    MaxFieldAlignment = maxAlignm,
                    Kind = LayoutKind.Sequential
                },
                Location = constantOrIncompleteArray.Location,
                IsNested = true,
                WriteCustomAttrs = static context => {
                    (var fieldDecl, var outputBuilder, var generator, var totalSizeString) = ((FieldDecl, IOutputBuilder, PInvokeGenerator, string?))context;

                    generator.WithAttributes(fieldDecl);
                    generator.WithUsings(fieldDecl);

                    if (totalSizeString is not null)
                    {
                        outputBuilder.WriteCustomAttribute($"InlineArray({totalSizeString})");
                    }
                },
                CustomAttrGeneratorData = (constantOrIncompleteArray, _outputBuilder, this, totalSizeString),
            };

            _outputBuilder.BeginStruct(in desc);

            var firstFieldName = "";
            var numFieldsToEmit = (totalSizeString is not null) ? Math.Min(totalSize, 1) : totalSize;

            for (long i = 0; i < numFieldsToEmit; i++)
            {
                var dimension = sizePerDimension[0];
                var firstDimension = dimension.index++;
                var fieldName = $"e{firstDimension}";
                sizePerDimension[0] = dimension;

                var separateStride = false;
                for (var d = 1; d < sizePerDimension.Count; d++)
                {
                    dimension = sizePerDimension[d];
                    fieldName += $"_{dimension.index}";
                    sizePerDimension[d] = dimension;

                    var previousDimension = sizePerDimension[d - 1];

                    if (previousDimension.index == previousDimension.size)
                    {
                        previousDimension.index = 0;
                        dimension.index++;
                        sizePerDimension[d - 1] = previousDimension;
                        separateStride = true;
                    }

                    sizePerDimension[d] = dimension;
                }

                if (string.IsNullOrEmpty(firstFieldName))
                {
                    firstFieldName = fieldName;
                }

                var fieldDesc = new FieldDesc {
                    AccessSpecifier = accessSpecifier,
                    NativeTypeName = null,
                    EscapedName = fieldName,
                    Offset = null,
                    NeedsNewKeyword = false,
                    Location = constantOrIncompleteArray.Location,
                };

                _outputBuilder.BeginField(in fieldDesc);
                _outputBuilder.WriteRegularField(arrayTypeName, fieldName);
                _outputBuilder.EndField(in fieldDesc);
                if (!separateStride)
                {
                    _outputBuilder.SuppressDivider();
                }
            }

            var generateCompatibleCode = _config.GenerateCompatibleCode;

            if (generateCompatibleCode || isUnsafeElementType)
            {
                _outputBuilder.BeginIndexer(AccessSpecifier.Public, isUnsafe: generateCompatibleCode && !isUnsafeElementType, needsUnscopedRef: false);
                _outputBuilder.WriteIndexer($"ref {arrayTypeName}");
                _outputBuilder.BeginIndexerParameters();
                var param = new ParameterDesc {
                    Name = "index",
                    Type = "int",
                };
                _outputBuilder.BeginParameter(in param);
                _outputBuilder.EndParameter(in param);
                _outputBuilder.EndIndexerParameters();
                _outputBuilder.BeginBody();

                _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining, isReadOnly: false);
                var code = _outputBuilder.BeginCSharpCode();

                code.WriteIndented("fixed (");
                code.Write(arrayTypeName);
                code.Write("* pThis = &");
                code.Write(firstFieldName);
                code.WriteLine(')');
                code.WriteBlockStart();
                code.WriteIndented("return ref pThis[index]");
                code.WriteSemicolon();
                code.WriteNewline();
                code.WriteBlockEnd();
                _outputBuilder.EndCSharpCode(code);

                _outputBuilder.EndGetter();
                _outputBuilder.EndBody();
                _outputBuilder.EndIndexer();
            }
            else if (totalSizeString is null)
            {
                _outputBuilder.BeginIndexer(AccessSpecifier.Public, isUnsafe: false, needsUnscopedRef: _config.GenerateLatestCode);
                _outputBuilder.WriteIndexer($"ref {arrayTypeName}");
                _outputBuilder.BeginIndexerParameters();
                var param = new ParameterDesc {
                    Name = "index",
                    Type = "int",
                };
                _outputBuilder.BeginParameter(in param);
                _outputBuilder.EndParameter(in param);
                _outputBuilder.EndIndexerParameters();
                _outputBuilder.BeginBody();

                _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining, isReadOnly: false);
                var code = _outputBuilder.BeginCSharpCode();
                code.AddUsingDirective("System");
                code.AddUsingDirective("System.Runtime.InteropServices");

                code.WriteIndented("return ref ");

                if (arraySize == 1)
                {
                    code.AddUsingDirective("System.Runtime.CompilerServices");
                    code.Write("Unsafe.Add(ref e0, index)");
                }
                else
                {
                    code.Write("AsSpan()[index]");
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);

                _outputBuilder.EndGetter();
                _outputBuilder.EndBody();
                _outputBuilder.EndIndexer();

                var function = new FunctionOrDelegateDesc {
                    AccessSpecifier = AccessSpecifier.Public,
                    EscapedName = "AsSpan",
                    IsAggressivelyInlined = _config.GenerateAggressiveInlining,
                    IsStatic = false,
                    IsMemberFunction = true,
                    ReturnType = $"Span<{arrayTypeName}>",
                    Location = constantOrIncompleteArray.Location,
                    HasBody = true,
                    NeedsUnscopedRef = _config.GenerateLatestCode,
                };

                var isUnsafe = false;
                _outputBuilder.BeginFunctionOrDelegate(in function, ref isUnsafe);

                _outputBuilder.BeginFunctionInnerPrototype(in function);

                if (arraySize == 1)
                {
                    param = new ParameterDesc {
                        Name = "length",
                        Type = "int",
                    };

                    _outputBuilder.BeginParameter(in param);
                    _outputBuilder.EndParameter(in param);
                }

                _outputBuilder.EndFunctionInnerPrototype(in function);
                _outputBuilder.BeginBody(true);
                code = _outputBuilder.BeginCSharpCode();

                code.Write("MemoryMarshal.CreateSpan(ref ");
                code.Write(firstFieldName);
                code.Write(", ");

                if (arraySize == 1)
                {
                    code.Write("length");
                }
                else
                {
                    code.Write(totalSize);
                }

                code.Write(')');
                code.WriteSemicolon();
                _outputBuilder.EndBody(true);
                _outputBuilder.EndCSharpCode(code);
                _outputBuilder.EndFunctionOrDelegate(in function);
            }

            _outputBuilder.EndStruct(in desc);
        }

        void StartTestOutput(ref bool testOutputStarted, bool generateTestsClass, bool isTopLevelStruct)
        {
            if ((_testOutputBuilder is not null) && generateTestsClass && !isTopLevelStruct && !testOutputStarted)
            {
                _testOutputBuilder.WriteIndented("/// <summary>Provides validation of the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> struct.</summary>");

                WithAttributes(recordDecl, onlySupportedOSPlatform: true, isTestOutput: true);

                _testOutputBuilder.WriteIndented("public static unsafe partial class ");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("Tests");
                _testOutputBuilder.WriteBlockStart();

                testOutputStarted = true;
            }
        }
    }

    private void VisitTranslationUnitDecl(TranslationUnitDecl translationUnitDecl)
    {
        Visit(translationUnitDecl.Decls);
        Visit(translationUnitDecl.CursorChildren, translationUnitDecl.Decls);
    }

    private void VisitTypedefDecl(TypedefDecl typedefDecl, bool onlyHandleRemappings)
    {
        ForUnderlyingType(typedefDecl, typedefDecl.UnderlyingType, onlyHandleRemappings);

        void ForFunctionProtoType(TypedefDecl typedefDecl, FunctionProtoType functionProtoType, Type? parentType, bool onlyHandleRemappings)
        {
            if (!_config.ExcludeFnptrCodegen || onlyHandleRemappings)
            {
                return;
            }

            var name = GetRemappedCursorName(typedefDecl);
            var escapedName = EscapeName(name);

            var callingConventionName = GetCallingConvention(typedefDecl, context: null, typedefDecl.TypeForDecl);

            var returnType = functionProtoType.ReturnType;
            var returnTypeName = GetRemappedTypeName(typedefDecl, context: null, returnType, out var nativeTypeName);

            StartUsingOutputBuilder(name);
            {
                Debug.Assert(_outputBuilder is not null);

                var desc = new FunctionOrDelegateDesc {
                    AccessSpecifier = GetAccessSpecifier(typedefDecl, matchStar: true),
                    CallingConvention = callingConventionName,
                    EscapedName = escapedName,
                    IsVirtual = true, // such that it outputs as a delegate
                    IsUnsafe = IsUnsafe(typedefDecl, functionProtoType),
                    NativeTypeName = nativeTypeName,
                    ReturnType = returnTypeName,
                    Location = typedefDecl.Location,
                    HasBody = true,
                    WriteCustomAttrs = static context => {
                        (var typedefDecl, var generator) = ((TypedefDecl, PInvokeGenerator))context;

                        generator.WithAttributes(typedefDecl);
                        generator.WithUsings(typedefDecl);
                    },
                    CustomAttrGeneratorData = (typedefDecl, this),
                };

                var isUnsafe = desc.IsUnsafe;
                _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);

                _outputBuilder.BeginFunctionInnerPrototype(in desc);

                Visit(typedefDecl.CursorChildren.OfType<ParmVarDecl>());

                _outputBuilder.EndFunctionInnerPrototype(in desc);
                _outputBuilder.EndFunctionOrDelegate(in desc);
            }
            StopUsingOutputBuilder();
        }

        void ForPointeeType(TypedefDecl typedefDecl, Type? parentType, Type pointeeType, bool onlyHandleRemappings)
        {
            if (IsType<FunctionProtoType>(typedefDecl, pointeeType, out var functionProtoType))
            {
                ForFunctionProtoType(typedefDecl, functionProtoType, parentType, onlyHandleRemappings);
            }
            else if (IsType<PointerType>(typedefDecl, pointeeType, out var pointerType))
            {
                ForPointeeType(typedefDecl, pointerType, pointerType.PointeeType, onlyHandleRemappings);
            }
        }

        void ForUnderlyingType(TypedefDecl typedefDecl, Type underlyingType, bool onlyHandleRemappings)
        {
            if (IsType<FunctionProtoType>(typedefDecl, underlyingType, out var functionProtoType))
            {
                ForFunctionProtoType(typedefDecl, functionProtoType, parentType: null, onlyHandleRemappings);
            }
            else if (IsType<PointerType>(typedefDecl, underlyingType, out var pointerType))
            {
                ForPointeeType(typedefDecl, parentType: null, pointerType.PointeeType, onlyHandleRemappings);
            }
            else if (IsType<ReferenceType>(typedefDecl, underlyingType, out var referenceType))
            {
                ForPointeeType(typedefDecl, parentType: null, referenceType.PointeeType, onlyHandleRemappings);
            }
            else if (IsType<TagType>(typedefDecl, underlyingType, out var tagType))
            {
                var tagDecl = tagType.AsTagDecl;
                Debug.Assert(tagDecl is not null);

                var underlyingName = GetCursorQualifiedName(tagDecl);
                var typedefName = GetCursorName(typedefDecl);

                if (underlyingName != typedefName)
                {
                    if (!_allValidNameRemappings.TryGetValue(underlyingName, out var allRemappings))
                    {
                        allRemappings = new HashSet<string>(QualifiedNameComparer.Default);
                        _allValidNameRemappings[underlyingName] = allRemappings;
                    }
                    _ = allRemappings.Add(typedefName);


                    if (!onlyHandleRemappings)
                    {
                        if (!_traversedValidNameRemappings.TryGetValue(underlyingName, out var traversedRemappings))
                        {
                            traversedRemappings = new HashSet<string>(QualifiedNameComparer.Default);
                            _traversedValidNameRemappings[underlyingName] = traversedRemappings;
                        }
                        _ = traversedRemappings.Add(typedefName);
                    }
                }
            }
            else if (IsType<TemplateSpecializationType>(typedefDecl, underlyingType, out var templateSpecializationType))
            {
                if (templateSpecializationType.IsTypeAlias)
                {
                    ForUnderlyingType(typedefDecl, templateSpecializationType.AliasedType, onlyHandleRemappings);
                }
            }
        }
    }

    private static void VisitUsingShadowDecl(UsingShadowDecl usingShadowDecl)
    {
        // Nothing to handle for binding generation
    }

    private void VisitVarDecl(VarDecl varDecl)
    {
        if (IsPrevContextStmt<DeclStmt>(out var declStmt, out _))
        {
            ForDeclStmt(varDecl, declStmt);
        }
        else if (IsPrevContextDecl<TranslationUnitDecl>(out _, out _) || IsPrevContextDecl<LinkageSpecDecl>(out _, out _) || IsPrevContextDecl<NamespaceDecl>(out _, out _) || IsPrevContextDecl<RecordDecl>(out _, out _))
        {
            if (!varDecl.HasInit)
            {
                // Nothing to do if a top level const declaration doesn't have an initializer
                return;
            }

            var type = varDecl.Type;
            var isMacroDefinitionRecord = false;

            var nativeName = GetCursorName(varDecl);
            if (nativeName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
            {
                type = varDecl.Init.Type;
                nativeName = nativeName["ClangSharpMacro_".Length..];
                isMacroDefinitionRecord = true;
            }

            var accessSpecifier = GetAccessSpecifier(varDecl, matchStar: false);
            var name = GetRemappedName(nativeName, varDecl, tryRemapOperatorName: false, out var wasRemapped, skipUsing: true);
            var escapedName = EscapeName(name);

            if (isMacroDefinitionRecord)
            {
                if (IsStmtAsWritten<DeclRefExpr>(varDecl.Init, out var declRefExpr, removeParens: true))
                {
                    if ((declRefExpr.Decl is NamedDecl namedDecl) && (name == GetCursorName(namedDecl)))
                    {
                        return;
                    }
                }
                else if (IsStmtAsWritten<RecoveryExpr>(varDecl.Init, out var recoveryExpr, removeParens: true))
                {
                    return;
                }
            }

            var openedOutputBuilder = false;
            var className = GetClass(name);

            if (_outputBuilder is null)
            {
                openedOutputBuilder = true;

                if (IsUnsafe(varDecl, type) && (!varDecl.HasInit || !IsStmtAsWritten<StringLiteral>(varDecl.Init, out _, removeParens: true)))
                {
                    _topLevelClassIsUnsafe[className] = true;
                }
            }

            var typeName = GetTargetTypeName(varDecl, out var nativeTypeName);

            if (isMacroDefinitionRecord)
            {
                var nativeTypeNameBuilder = new StringBuilder("#define");

                _ = nativeTypeNameBuilder.Append(' ');
                _ = nativeTypeNameBuilder.Append(nativeName);
                _ = nativeTypeNameBuilder.Append(' ');

                var macroValue = GetSourceRangeContents(varDecl.TranslationUnit.Handle, varDecl.Init.Extent);
                _ = nativeTypeNameBuilder.Append(macroValue.Replace(@"\\", "\\", StringComparison.Ordinal));

                nativeTypeName = nativeTypeNameBuilder.ToString();
            }

            var kind = ValueKind.Unknown;
            var flags = ValueFlags.None;

            if (varDecl.HasInit)
            {
                flags |= ValueFlags.Initializer;
            }

            if (type.IsLocalConstQualified || isMacroDefinitionRecord || IsTypeConstantOrIncompleteArray(varDecl))
            {
                flags |= ValueFlags.Constant;
            }

            if (IsStmtAsWritten<StringLiteral>(varDecl.Init, out var stringLiteral, removeParens: true))
            {
                kind = ValueKind.String;

                switch (stringLiteral.Kind)
                {
                    case CX_SLK_Ordinary:
                    case CX_SLK_UTF8:
                    {
                        typeName = flags.HasFlag(ValueFlags.Constant) ? "ReadOnlySpan<byte>" : "byte[]";
                        break;
                    }

                    case CX_SLK_Wide:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto case CX_SLK_UTF32;
                        }
                        else
                        {
                            goto case CX_SLK_UTF16;
                        }
                    }

                    case CX_SLK_UTF16:
                    {
                        kind = ValueKind.Primitive;
                        typeName = "string";
                        break;
                    }

                    case CX_SLK_UTF32:
                    {
                        typeName = (_config.GenerateLatestCode && flags.HasFlag(ValueFlags.Constant)) ? "ReadOnlySpan<uint>" : "uint[]";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Error, $"Unsupported string literal kind: '{stringLiteral.Kind}'. Generated bindings may be incomplete.", stringLiteral);
                        break;
                    }
                }
            }
            else if (IsPrimitiveValue(varDecl, type))
            {
                kind = ValueKind.Primitive;

                if (flags.HasFlag(ValueFlags.Constant) && !IsConstant(typeName, varDecl.Init))
                {
                    flags |= ValueFlags.Copy;
                }
                else if (_config.WithTransparentStructs.TryGetValue(typeName, out var transparentStruct))
                {
                    typeName = transparentStruct.Name;
                }
            }
            else if ((varDecl.StorageClass == CX_SC_Static) || openedOutputBuilder)
            {
                kind = ValueKind.Unmanaged;

                if (varDecl.HasInit)
                {
                    if ((varDecl.Init is CXXConstructExpr cxxConstructExpr) && cxxConstructExpr.Constructor.IsCopyConstructor)
                    {
                        if (cxxConstructExpr.Args[0] is CXXUuidofExpr)
                        {
                            // It's easiest just to let _uuidsToGenerate handle it
                            return;
                        }
                        flags |= ValueFlags.Copy;
                    }
                }

                if (IsType<ArrayType>(varDecl, type, out var arrayType))
                {
                    flags |= ValueFlags.Array;

                    if (!_config.GenerateUnmanagedConstants)
                    {
                        do
                        {
                            typeName += "[]";
                            arrayType = arrayType.ElementType as ArrayType;
                        }
                        while (arrayType is not null);
                    }
                }
            }

            if (typeName.Equals("Guid", StringComparison.Ordinal))
            {
                _ = _generatedUuids.Add(name);
            }

            var desc = new ValueDesc {
                AccessSpecifier = accessSpecifier,
                TypeName = typeName,
                EscapedName = escapedName,
                NativeTypeName = nativeTypeName,
                Kind = kind,
                Flags = flags,
                Location = varDecl.Location,
                WriteCustomAttrs = static context => {
                    (var varDecl, var generator) = ((VarDecl, PInvokeGenerator))context;

                    generator.WithAttributes(varDecl);
                    generator.WithUsings(varDecl);
                },
                CustomAttrGeneratorData = (varDecl, this),
            };

            if (openedOutputBuilder)
            {
                StartUsingOutputBuilder(className);
                Debug.Assert(_outputBuilder is not null);

                if ((kind == ValueKind.String) && typeName.StartsWith("ReadOnlySpan<", StringComparison.Ordinal))
                {
                    _outputBuilder.EmitSystemSupport();
                }
            }

            Debug.Assert(_outputBuilder is not null);

            _outputBuilder.BeginValue(in desc);

            var currentContext = _context.Last;
            Debug.Assert(currentContext is not null);
            currentContext.Value = (currentContext.Value.Cursor, desc);

            if (varDecl.HasInit)
            {
                var dereference = IsType<PointerType>(varDecl, type, out var pointerType) &&
                                  IsType<FunctionType>(varDecl, pointerType.PointeeType) &&
                                  isMacroDefinitionRecord;

                if (dereference)
                {
                    _outputBuilder.BeginDereference();
                }

                Visit(varDecl.Init);

                if (dereference)
                {
                    _outputBuilder.EndDereference();
                }
            }

            _outputBuilder.EndValue(in desc);

            if (openedOutputBuilder)
            {
                StopUsingOutputBuilder();
            }
            else
            {
                _outputBuilder.WriteDivider();
            }
        }
        else if (IsPrevContextDecl<FunctionDecl>(out _, out _))
        {
            // This should be handled in the function body as part of a DeclStmt
        }
        else
        {
            _ = IsPrevContextDecl<Decl>(out var previousContext, out _);
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported variable declaration parent: '{previousContext?.CursorKindSpelling}'. Generated bindings may be incomplete.", previousContext);
        }

        void ForDeclStmt(VarDecl varDecl, DeclStmt declStmt)
        {
            var outputBuilder = StartCSharpCode();
            var name = GetRemappedCursorName(varDecl);
            var escapedName = EscapeName(name);

            if (varDecl == declStmt.Decls[0])
            {
                var type = varDecl.Type;
                var typeName = GetRemappedTypeName(varDecl, context: null, type, out _);

                outputBuilder.Write(typeName);

                if (IsType<ArrayType>(varDecl, type))
                {
                    outputBuilder.Write("[]");
                }

                outputBuilder.Write(' ');
            }

            outputBuilder.Write(escapedName);

            if (varDecl.HasInit)
            {
                outputBuilder.Write(" = ");
                Visit(varDecl.Init);
            }

            StopCSharpCode();
        }
    }

    private bool IsConstant(string targetTypeName, Expr initExpr)
    {
        if (IsTypePointerOrReference(initExpr) && !targetTypeName.Equals("string", StringComparison.Ordinal))
        {
            return false;
        }

        switch (initExpr.StmtClass)
        {
            // case CX_StmtClass_BinaryConditionalOperator:

            case CX_StmtClass_ConditionalOperator:
            {
                var conditionalOperator = (ConditionalOperator)initExpr;
                return IsConstant(targetTypeName, conditionalOperator.Cond) && IsConstant(targetTypeName, conditionalOperator.LHS) && IsConstant(targetTypeName, conditionalOperator.RHS);
            }

            // case CX_StmtClass_AddrLabelExpr:
            // case CX_StmtClass_ArrayInitIndexExpr:
            // case CX_StmtClass_ArrayInitLoopExpr:

            case CX_StmtClass_ArraySubscriptExpr:
            {
                return false;
            }

            // case CX_StmtClass_ArrayTypeTraitExpr:
            // case CX_StmtClass_AsTypeExpr:
            // case CX_StmtClass_AtomicExpr:

            case CX_StmtClass_BinaryOperator:
            {
                var binaryOperator = (BinaryOperator)initExpr;
                return IsConstant(targetTypeName, binaryOperator.LHS) && IsConstant(targetTypeName, binaryOperator.RHS);
            }

            // case CX_StmtClass_CompoundAssignOperator:
            // case CX_StmtClass_BlockExpr:
            // case CX_StmtClass_CXXBindTemporaryExpr:

            case CX_StmtClass_CXXBoolLiteralExpr:
            {
                return true;
            }

            case CX_StmtClass_CXXConstructExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXTemporaryObjectExpr:
            // case CX_StmtClass_CXXDefaultArgExpr:

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
                return true;
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
                var callExpr = (CallExpr)initExpr;

                var directCallee = callExpr.DirectCallee;
                Debug.Assert(directCallee is not null);

                if (directCallee.IsInlined)
                {
                    var evaluateResult = callExpr.Handle.Evaluate;

                    switch (evaluateResult.Kind)
                    {
                        case CXEval_Int:
                        {
                            return true;
                        }

                        case CXEval_Float:
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            // case CX_StmtClass_CUDAKernelCallExpr:
            // case CX_StmtClass_CXXMemberCallExpr:

            case CX_StmtClass_CXXOperatorCallExpr:
            {
                var cxxOperatorCall = (CXXOperatorCallExpr)initExpr;

                if (cxxOperatorCall.CalleeDecl is FunctionDecl functionDecl)
                {
                    var functionDeclName = GetCursorName(functionDecl);

                    if (IsEnumOperator(functionDecl, functionDeclName))
                    {
                        return true;
                    }
                }

                return false;
            }

            // case CX_StmtClass_UserDefinedLiteral:
            // case CX_StmtClass_BuiltinBitCastExpr:

            case CX_StmtClass_CStyleCastExpr:
            case CX_StmtClass_CXXStaticCastExpr:
            case CX_StmtClass_CXXFunctionalCastExpr:
            case CX_StmtClass_CXXConstCastExpr:
            case CX_StmtClass_CXXDynamicCastExpr:
            case CX_StmtClass_CXXReinterpretCastExpr:
            {
                var cxxFunctionalCastExpr = (ExplicitCastExpr)initExpr;
                return IsConstant(targetTypeName, cxxFunctionalCastExpr.SubExprAsWritten);
            }

            // case CX_StmtClass_ObjCBridgedCastExpr:

            case CX_StmtClass_ImplicitCastExpr:
            {
                var implicitCastExpr = (ImplicitCastExpr)initExpr;
                return IsConstant(targetTypeName, implicitCastExpr.SubExprAsWritten);
            }

            case CX_StmtClass_CharacterLiteral:
            {
                return true;
            }

            // case CX_StmtClass_ChooseExpr:
            // case CX_StmtClass_CompoundLiteralExpr:
            // case CX_StmtClass_ConceptSpecializationExpr:
            // case CX_StmtClass_ConvertVectorExpr:
            // case CX_StmtClass_CoawaitExpr:
            // case CX_StmtClass_CoyieldExpr:

            case CX_StmtClass_DeclRefExpr:
            {
                var declRefExpr = (DeclRefExpr)initExpr;
                return (declRefExpr.Decl is EnumConstantDecl) ||
                       ((declRefExpr.Decl is VarDecl varDecl) && varDecl.HasInit && IsConstant(targetTypeName, varDecl.Init));
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
                return true;
            }

            // case CX_StmtClass_ConstantExpr:

            case CX_StmtClass_ExprWithCleanups:
            {
                var exprWithCleanups = (ExprWithCleanups)initExpr;
                return IsConstant(targetTypeName, exprWithCleanups.SubExpr);
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
                return true;
            }

            case CX_StmtClass_LambdaExpr:
            {
                return false;
            }

            // case CX_StmtClass_MSPropertyRefExpr:
            // case CX_StmtClass_MSPropertySubscriptExpr:
            // case CX_StmtClass_MaterializeTemporaryExpr:

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
                var parenExpr = (ParenExpr)initExpr;
                return IsConstant(targetTypeName, parenExpr.SubExpr);
            }

            case CX_StmtClass_ParenListExpr:
            {
                var parenListExpr = (ParenListExpr)initExpr;

                foreach (var expr in parenListExpr.Exprs)
                {
                    if (IsConstant(targetTypeName, expr))
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
                return true;
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
                var unaryExprOrTypeTraitExpr = (UnaryExprOrTypeTraitExpr)initExpr;
                var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;


                long alignment32 = -1;
                long alignment64 = -1;

                GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out var size32, out var size64);

                switch (unaryExprOrTypeTraitExpr.Kind)
                {
                    case CX_UETT_SizeOf:
                    {
                        return size32 == size64;
                    }

                    case CX_UETT_AlignOf:
                    case CX_UETT_PreferredAlignOf:
                    {
                        return alignment32 == alignment64;
                    }

                    default:
                    {
                        return false;
                    }
                }
            }

            case CX_StmtClass_UnaryOperator:
            {
                var unaryOperator = (UnaryOperator)initExpr;
                return IsConstant(targetTypeName, unaryOperator.SubExpr)
                    && ((unaryOperator.Opcode != CXUnaryOperator_Minus) || (targetTypeName is not "IntPtr" and not "nint" and not "nuint" and not "UIntPtr"));
            }

            // case CX_StmtClass_VAArgExpr:

            default:
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported statement class: '{initExpr.StmtClassName}'. Generated bindings may not be constant.", initExpr);
                return false;
            }
        }
    }

    private static bool IsPrimitiveValue(Cursor? cursor, Type type)
    {
        if (IsType<BuiltinType>(cursor, type, out var builtinType))
        {
            switch (builtinType.Kind)
            {
                case CXType_Bool:
                case CXType_Char_U:
                case CXType_UChar:
                case CXType_Char16:
                case CXType_UShort:
                case CXType_UInt:
                case CXType_ULong:
                case CXType_ULongLong:
                case CXType_Char_S:
                case CXType_SChar:
                case CXType_WChar:
                case CXType_Short:
                case CXType_Int:
                case CXType_Long:
                case CXType_LongLong:
                case CXType_Float:
                case CXType_Double:
                {
                    return true;
                }
            }
        }
        else if (IsType<EnumType>(cursor, type, out var enumType))
        {
            return IsPrimitiveValue(cursor, enumType.Decl.IntegerType);
        }

        return IsTypePointerOrReference(cursor, type);
    }
}
