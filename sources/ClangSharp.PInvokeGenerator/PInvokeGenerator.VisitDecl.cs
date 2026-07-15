// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using static ClangSharp.Interop.CX_CastKind;
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

    private void TransformBoolType(ref string typeName, ref string nativeTypeName)
    {
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
    }

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

            if (IsAnonymousEnum(parentName))
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

        var escapedName = EscapeAndStripEnumMemberName(name, parentName, enumConstantDecl.DeclContext as EnumDecl);

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

        if (IsAnonymousEnum(name))
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

                        generator.WithAttributes(enumDecl, emitGeneratedCodeAttribute: true);
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

        TransformBoolType(ref typeName, ref nativeTypeName);

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
            CppAttributes = _config.GenerateCppAttributes
                ? fieldDecl.Attrs.Select(x => EscapeString(x.Spelling))
                : null,
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
            TransformBoolType(ref returnTypeName, ref nativeTypeName);
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
            IsReadOnly = IsReadonly(cxxMethodDecl),
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

                var cxxBaseSpecifier = GetBaseSubobjectSpecifier(_cxxRecordDeclContext, parent);

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

        if (IsPrevContextDecl<RecordDecl>(out var prevContext, out _) && prevContext.IsAnonymousStructOrUnion)
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

        while (rootRecordDecl.IsAnonymousStructOrUnion && (rootRecordDecl.Parent is RecordDecl parentRecordDecl))
        {
            // The name of a field of an anonymous type should be same as the type's name minus the
            // type kind tag at the end and the leading `_`.
            var contextNamePart = GetRemappedCursorName(rootRecordDecl);
            var tagIndex = contextNamePart.LastIndexOf(AnonymousTypeKindTag, StringComparison.Ordinal);
            Debug.Assert(contextNamePart[0] == '_');
            Debug.Assert(tagIndex >= 0);
            contextNamePart = contextNamePart[1..tagIndex];

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

        var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out var nativeTypeName);

        TransformBoolType(ref typeName, ref nativeTypeName);

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
            NeedsUnscopedRef = !_config.GenerateCompatibleCode && !fieldDecl.IsBitField,
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
                    else if (_config.GenerateCompatibleCode || arraySize == 1)
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

    private void VisitTranslationUnitDecl(TranslationUnitDecl translationUnitDecl)
    {
        PreprocessUuidNameOverrides(translationUnitDecl.Decls);

        Visit(translationUnitDecl.Decls);
        Visit(translationUnitDecl.CursorChildren, translationUnitDecl.Decls);
    }

    private static readonly string[] s_uuidNamePrefixes = ["CLSID_", "IID_", "LIBID_", "FMTID_", "CATID_", "GUID_"];

    private void PreprocessUuidNameOverrides(IEnumerable<Decl> decls)
    {
        // A top-level `const GUID CLSID_Foo;` (extern, no initializer) names, by COM convention, the
        // uuid of the adjacent `DECLSPEC_UUID`-annotated record `Foo`. Its value is only knowable from
        // that record's uuid attribute, so we record a mapping from the record's native name to the
        // global's declared name and use it as the record's uuid-constant name during VisitRecordDecl.
        // This keeps exactly one constant (e.g. `CLSID_Foo`) and points the GuidOfTest at it.

        foreach (var decl in decls)
        {
            if (decl is LinkageSpecDecl linkageSpecDecl)
            {
                PreprocessUuidNameOverrides(linkageSpecDecl.Decls);
                continue;
            }
            else if (decl is NamespaceDecl namespaceDecl)
            {
                PreprocessUuidNameOverrides(namespaceDecl.Decls);
                continue;
            }

            if (decl is not VarDecl varDecl || varDecl.HasInit || !varDecl.Type.IsLocalConstQualified)
            {
                continue;
            }

            // Resolve the managed type via the canonical record decl name rather than the full type
            // string machinery, which is not safe to run during this pre-scan (it happens before normal
            // visitation and can fault on incomplete/deduced types).
            if (varDecl.Type.CanonicalType is not RecordType recordType || !GetRemappedCursorName(recordType.Decl).Equals("Guid", StringComparison.Ordinal))
            {
                continue;
            }

            var nativeName = GetCursorName(varDecl);

            foreach (var prefix in s_uuidNamePrefixes)
            {
                if ((nativeName.Length <= prefix.Length) || !nativeName.StartsWith(prefix, StringComparison.Ordinal))
                {
                    continue;
                }

                var associatedNativeName = nativeName[prefix.Length..];
                var remappedName = GetRemappedName(nativeName, varDecl, tryRemapOperatorName: false, out _, skipUsing: true);

                // Prefer an `IID_` global when several globals map to the same record.
                if (!_uuidNameOverrides.ContainsKey(associatedNativeName) || prefix.Equals("IID_", StringComparison.Ordinal))
                {
                    _uuidNameOverrides[associatedNativeName] = remappedName;
                }

                break;
            }
        }
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

                        generator.WithAttributes(typedefDecl, emitGeneratedCodeAttribute: true);
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

    private bool IsConstant(string targetTypeName, Expr initExpr)
    {
        // Constant expressions for native integers must be in range of the corresponding 32-bit integer type
        // Also see: https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/native-integers.md
        if ((targetTypeName is "nuint" or "UIntPtr" && initExpr.Handle.Evaluate is { Kind: CXEval_Int, AsUnsigned: > uint.MaxValue })
             || (targetTypeName is "nint" or "IntPtr" && initExpr.Handle.Evaluate is { Kind: CXEval_Int, AsLongLong: < int.MinValue or > int.MaxValue }))
        {
            return false;
        }

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
