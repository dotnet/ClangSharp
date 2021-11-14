// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ClangSharp.Abstractions;
using ClangSharp.Interop;

namespace ClangSharp
{
    public partial class PInvokeGenerator
    {
        private void VisitClassTemplateDecl(ClassTemplateDecl classTemplateDecl) => Visit(classTemplateDecl.TemplatedDecl);

        private void VisitClassTemplateSpecializationDecl(ClassTemplateSpecializationDecl classTemplateSpecializationDecl) => AddDiagnostic(DiagnosticLevel.Warning, $"Class template specializations are not supported: '{GetCursorQualifiedName(classTemplateSpecializationDecl)}'. Generated bindings may be incomplete.", classTemplateSpecializationDecl);

        private void VisitDecl(Decl decl)
        {
            if (IsExcluded(decl))
            {
                if (decl.Kind == CX_DeclKind.CX_DeclKind_Typedef)
                {
                    VisitTypedefDecl((TypedefDecl)decl, onlyHandleRemappings: true);
                }
                return;
            }

            switch (decl.Kind)
            {
                case CX_DeclKind.CX_DeclKind_AccessSpec:
                {
                    // Access specifications are also exposed as a queryable property
                    // on the declarations they impact, so we don't need to do anything
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_Block:
                // case CX_DeclKind.CX_DeclKind_Captured:
                // case CX_DeclKind.CX_DeclKind_ClassScopeFunctionSpecialization:

                case CX_DeclKind.CX_DeclKind_Empty:
                {
                    // Nothing to generate for empty declarations
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_Export:
                // case CX_DeclKind.CX_DeclKind_ExternCContext:
                // case CX_DeclKind.CX_DeclKind_FileScopeAsm:

                case CX_DeclKind.CX_DeclKind_Friend:
                {
                    // Nothing to generate for friend declarations
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_FriendTemplate:
                // case CX_DeclKind.CX_DeclKind_Import:

                case CX_DeclKind.CX_DeclKind_LinkageSpec:
                {
                    VisitLinkageSpecDecl((LinkageSpecDecl)decl);
                    break;
                }

                case CX_DeclKind.CX_DeclKind_Label:
                {
                    VisitLabelDecl((LabelDecl)decl);
                    break;
                }

                case CX_DeclKind.CX_DeclKind_Namespace:
                {
                    VisitNamespaceDecl((NamespaceDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_NamespaceAlias:
                // case CX_DeclKind.CX_DeclKind_ObjCCompatibleAlias:
                // case CX_DeclKind.CX_DeclKind_ObjCCategory:
                // case CX_DeclKind.CX_DeclKind_ObjCCategoryImpl:
                // case CX_DeclKind.CX_DeclKind_ObjCImplementation:
                // case CX_DeclKind.CX_DeclKind_ObjCInterface:
                // case CX_DeclKind.CX_DeclKind_ObjCProtocol:
                // case CX_DeclKind.CX_DeclKind_ObjCMethod:
                // case CX_DeclKind.CX_DeclKind_ObjCProperty:
                // case CX_DeclKind.CX_DeclKind_BuiltinTemplate:
                // case CX_DeclKind.CX_DeclKind_Concept:

                case CX_DeclKind.CX_DeclKind_ClassTemplate:
                {
                    VisitClassTemplateDecl((ClassTemplateDecl)decl);
                    break;
                }

                case CX_DeclKind.CX_DeclKind_FunctionTemplate:
                {
                    VisitFunctionTemplateDecl((FunctionTemplateDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_TypeAliasTemplate:
                // case CX_DeclKind.CX_DeclKind_VarTemplate:
                // case CX_DeclKind.CX_DeclKind_TemplateTemplateParm:

                case CX_DeclKind.CX_DeclKind_Enum:
                {
                    VisitEnumDecl((EnumDecl)decl);
                    break;
                }

                case CX_DeclKind.CX_DeclKind_Record:
                case CX_DeclKind.CX_DeclKind_CXXRecord:
                {
                    VisitRecordDecl((RecordDecl)decl);
                    break;
                }

                case CX_DeclKind.CX_DeclKind_ClassTemplateSpecialization:
                {
                    VisitClassTemplateSpecializationDecl((ClassTemplateSpecializationDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_ClassTemplatePartialSpecialization:
                // case CX_DeclKind.CX_DeclKind_TemplateTypeParm:
                // case CX_DeclKind.CX_DeclKind_ObjCTypeParam:

                case CX_DeclKind.CX_DeclKind_TypeAlias:
                {
                    VisitTypeAliasDecl((TypeAliasDecl)decl);
                    break;
                }

                case CX_DeclKind.CX_DeclKind_Typedef:
                {
                    VisitTypedefDecl((TypedefDecl)decl, onlyHandleRemappings: false);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_UnresolvedUsingTypename:

                case CX_DeclKind.CX_DeclKind_Using:
                {
                    // Using declarations only introduce existing members into
                    // the current scope. There isn't an easy way to translate
                    // this to C#, so we will ignore them for now.
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_UsingDirective:
                // case CX_DeclKind.CX_DeclKind_UsingPack:

                case CX_DeclKind.CX_DeclKind_UsingShadow:
                {
                    VisitUsingShadowDecl((UsingShadowDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_ConstructorUsingShadow:
                // case CX_DeclKind.CX_DeclKind_Binding:

                case CX_DeclKind.CX_DeclKind_Field:
                {
                    VisitFieldDecl((FieldDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_ObjCAtDefsField:
                // case CX_DeclKind.CX_DeclKind_ObjCIvar:

                case CX_DeclKind.CX_DeclKind_Function:
                case CX_DeclKind.CX_DeclKind_CXXMethod:
                case CX_DeclKind.CX_DeclKind_CXXConstructor:
                case CX_DeclKind.CX_DeclKind_CXXDestructor:
                case CX_DeclKind.CX_DeclKind_CXXConversion:
                {
                    VisitFunctionDecl((FunctionDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_CXXDeductionGuide:
                // case CX_DeclKind.CX_DeclKind_MSProperty:
                // case CX_DeclKind.CX_DeclKind_NonTypeTemplateParm:

                case CX_DeclKind.CX_DeclKind_Var:
                {
                    VisitVarDecl((VarDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_Decomposition:
                // case CX_DeclKind.CX_DeclKind_ImplicitParam:
                // case CX_DeclKind.CX_DeclKind_OMPCapturedExpr:

                case CX_DeclKind.CX_DeclKind_ParmVar:
                {
                    VisitParmVarDecl((ParmVarDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_VarTemplateSpecialization:
                // case CX_DeclKind.CX_DeclKind_VarTemplatePartialSpecialization:

                case CX_DeclKind.CX_DeclKind_EnumConstant:
                {
                    VisitEnumConstantDecl((EnumConstantDecl)decl);
                    break;
                }

                case CX_DeclKind.CX_DeclKind_IndirectField:
                {
                    VisitIndirectFieldDecl((IndirectFieldDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_OMPDeclareMapper:
                // case CX_DeclKind.CX_DeclKind_OMPDeclareReduction:
                // case CX_DeclKind.CX_DeclKind_UnresolvedUsingValue:
                // case CX_DeclKind.CX_DeclKind_OMPAllocate:
                // case CX_DeclKind.CX_DeclKind_OMPRequires:
                // case CX_DeclKind.CX_DeclKind_OMPThreadPrivate:
                // case CX_DeclKind.CX_DeclKind_ObjCPropertyImpl:

                case CX_DeclKind.CX_DeclKind_PragmaComment:
                {
                    // Pragma comments can't be easily modeled in C#
                    // We'll ignore them for now.
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_PragmaDetectMismatch:

                case CX_DeclKind.CX_DeclKind_StaticAssert:
                {
                    // Static asserts can't be easily modeled in C#
                    // We'll ignore them for now.
                    break;
                }

                case CX_DeclKind.CX_DeclKind_TranslationUnit:
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
            var accessSpecifier = AccessSpecifier.None;
            var name = GetRemappedCursorName(enumConstantDecl);
            var escapedName = EscapeName(name);
            var typeName = GetTargetTypeName(enumConstantDecl, out _);
            var isAnonymousEnum = false;

            if ((enumConstantDecl.DeclContext is EnumDecl enumDecl) && GetRemappedCursorName(enumDecl).StartsWith("__AnonymousEnum_"))
            {
                isAnonymousEnum = true;
                accessSpecifier = GetAccessSpecifier(enumDecl);
            }

            var kind = isAnonymousEnum ? ValueKind.Primitive : ValueKind.Enumerator;
            var flags = ValueFlags.Constant;

            if ((enumConstantDecl.InitExpr is not null) || isAnonymousEnum)
            {
                flags |= ValueFlags.Initializer;
            }

            var desc = new ValueDesc
            {
                AccessSpecifier = accessSpecifier,
                TypeName = typeName,
                EscapedName = escapedName,
                NativeTypeName = null,
                Kind = kind,
                Flags = flags,
                Location = enumConstantDecl.Location
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
            var accessSpecifier = GetAccessSpecifier(enumDecl);
            var name = GetRemappedCursorName(enumDecl);
            var escapedName = EscapeName(name);
            var isAnonymousEnum = false;

            if (name.StartsWith("__AnonymousEnum_"))
            {
                isAnonymousEnum = true;
                name = GetClass(name);
            }

            StartUsingOutputBuilder(name);
            {
                if (!isAnonymousEnum)
                {
                    var typeName = GetRemappedTypeName(enumDecl, context: null, enumDecl.IntegerType, out var nativeTypeName);

                    var desc = new EnumDesc()
                    {
                        AccessSpecifier = accessSpecifier,
                        TypeName = typeName,
                        EscapedName = escapedName,
                        NativeType = nativeTypeName,
                        Location = enumDecl.Location,
                        IsNested = enumDecl.DeclContext is TagDecl,
                    };

                    _outputBuilder.BeginEnum(in desc);
                }

                Visit(enumDecl.Enumerators);
                Visit(enumDecl.Decls, excludedCursors: enumDecl.Enumerators);

                if (!isAnonymousEnum)
                {
                    _outputBuilder.EndEnum();
                }
            }
            StopUsingOutputBuilder();
        }

        private void VisitFieldDecl(FieldDecl fieldDecl)
        {
            if (fieldDecl.IsBitField)
            {
                return;
            }

            var accessSpecifier = GetAccessSpecifier(fieldDecl);
            var name = GetRemappedCursorName(fieldDecl);
            var escapedName = EscapeName(name);

            var type = fieldDecl.Type;
            var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out var nativeTypeName);

            int? offset = null;
            if (fieldDecl.Parent.IsUnion)
            {
                offset = 0;
            }

            var desc = new FieldDesc
            {
                AccessSpecifier = accessSpecifier,
                NativeTypeName = nativeTypeName,
                EscapedName = escapedName,
                Offset = offset,
                NeedsNewKeyword = NeedsNewKeyword(name),
                Location = fieldDecl.Location
            };

            _outputBuilder.BeginField(in desc);

            if (type.CanonicalType is ConstantArrayType constantArrayType)
            {
                var count = Math.Max(constantArrayType.Size, 1).ToString();
                var elementType = constantArrayType.ElementType;
                while (elementType.CanonicalType is ConstantArrayType subConstantArrayType)
                {
                    count += " * ";
                    count += Math.Max(subConstantArrayType.Size, 1).ToString();
                    elementType = subConstantArrayType.ElementType;
                }

                _outputBuilder.WriteFixedCountField(typeName, escapedName, GetArtificialFixedSizedBufferName(fieldDecl), count);
            }
            else
            {
                _outputBuilder.WriteRegularField(typeName, escapedName);
            }

            _outputBuilder.EndField();
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
            var className = name;

            if (functionDecl.DeclContext is not CXXRecordDecl cxxRecordDecl)
            {
                cxxRecordDecl = null;
                className = GetClass(name);
                StartUsingOutputBuilder(className);
            }
            else if ((Cursor)functionDecl.LexicalDeclContext != cxxRecordDecl)
            {
                // We shouldn't reprocess C++ functions outside the declaration
                return;
            }

            var accessSppecifier = GetAccessSpecifier(functionDecl);

            var cxxMethodDecl = functionDecl as CXXMethodDecl;
            var body = functionDecl.Body;

            var isVirtual = (cxxMethodDecl != null) && cxxMethodDecl.IsVirtual;
            var escapedName = isVirtual ? PrefixAndStripName(name, GetOverloadIndex(cxxMethodDecl)) : EscapeAndStripName(name);

            var returnType = functionDecl.ReturnType;
            var returnTypeName = GetRemappedTypeName(functionDecl, cxxRecordDecl, returnType, out var nativeTypeName);

            if ((isVirtual || (body is null)) && (returnTypeName == "bool"))
            {
                // bool is not blittable, so we shouldn't use it for P/Invoke signatures
                returnTypeName = "byte";
                nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? "bool" : nativeTypeName;
            }

            var type = functionDecl.Type;
            var callingConventionName = GetCallingConvention(functionDecl, cxxRecordDecl, type);

            var isDllImport = body is null && !isVirtual;
            var entryPoint = "";

            if (isDllImport)
            {
                entryPoint = functionDecl.IsExternC ? GetCursorName(functionDecl) : functionDecl.Handle.Mangling.CString;
            }

            var needsReturnFixup = isVirtual && NeedsReturnFixup(cxxMethodDecl);

            var desc = new FunctionOrDelegateDesc<(string Name, PInvokeGenerator This)>
            {
                AccessSpecifier = accessSppecifier,
                NativeTypeName = nativeTypeName,
                EscapedName = escapedName,
                EntryPoint = entryPoint,
                CallingConvention = callingConventionName,
                LibraryPath = isDllImport ? GetLibraryPath(name).Unquote() : null,
                IsVirtual = isVirtual,
                IsDllImport = isDllImport,
                HasFnPtrCodeGen = !_config.ExcludeFnptrCodegen,
                SetLastError = GetSetLastError(name),
                IsCxx = cxxMethodDecl is not null,
                IsStatic = isDllImport || (cxxMethodDecl?.IsStatic ?? true),
                NeedsNewKeyword = NeedsNewKeyword(escapedName, functionDecl.Parameters),
                IsUnsafe = IsUnsafe(functionDecl),
                IsCtxCxxRecord = cxxRecordDecl is not null,
                IsCxxRecordCtxUnsafe = cxxRecordDecl is not null && IsUnsafe(cxxRecordDecl),
                WriteCustomAttrs = static x =>
                {
                    x.This.WithAttributes("*");
                    x.This.WithAttributes(x.Name);

                    x.This.WithUsings("*");
                    x.This.WithUsings(x.Name);
                },
                CustomAttrGeneratorData = (name, this),
                NeedsReturnFixup = needsReturnFixup,
                ReturnType = needsReturnFixup ? $"{returnTypeName}*" : returnTypeName,
                IsCxxConstructor = functionDecl is CXXConstructorDecl,
                Location = functionDecl.Location
            };

            _ = _isTopLevelClassUnsafe.TryGetValue(className, out var isUnsafe);
            _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);
            _isTopLevelClassUnsafe[className] = isUnsafe;

            _outputBuilder.BeginFunctionInnerPrototype(escapedName);

            if (isVirtual)
            {
                Debug.Assert(cxxRecordDecl != null);

                if (!IsPrevContextDecl<CXXRecordDecl>(out var thisCursor, out _))
                {
                    thisCursor = cxxRecordDecl;
                }

                var cxxRecordDeclName = GetRemappedCursorName(thisCursor);
                var cxxRecordEscapedName = EscapeName(cxxRecordDeclName);
                ParameterDesc<(string Name, PInvokeGenerator This)> parameterDesc = new()
                {
                    Name = "pThis",
                    Type = $"{cxxRecordEscapedName}*"
                };

                _outputBuilder.BeginParameter(in parameterDesc);
                _outputBuilder.EndParameter();

                if (needsReturnFixup)
                {
                    _outputBuilder.WriteParameterSeparator();
                    parameterDesc = new()
                    {
                        Name = "_result",
                        Type = $"{returnTypeName}*"
                    };
                    _outputBuilder.BeginParameter(in parameterDesc);
                    _outputBuilder.EndParameter();
                }

                if (functionDecl.Parameters.Any())
                {
                    _outputBuilder.WriteParameterSeparator();
                }
            }

            Visit(functionDecl.Parameters);

            _outputBuilder.EndFunctionInnerPrototype();

            if ((body is null) || isVirtual)
            {
                _outputBuilder.EndFunctionOrDelegate(isVirtual, true);
            }
            else
            {
                var firstCtorInitializer = functionDecl.Parameters.Any() ? (functionDecl.CursorChildren.IndexOf(functionDecl.Parameters.Last()) + 1) : 0;
                var lastCtorInitializer = (functionDecl.Body != null) ? functionDecl.CursorChildren.IndexOf(functionDecl.Body) : functionDecl.CursorChildren.Count;

                _outputBuilder.BeginBody();

                if (functionDecl is CXXConstructorDecl cxxConstructorDecl)
                {
                    VisitCtorInitializers(cxxConstructorDecl, firstCtorInitializer, lastCtorInitializer);
                }

                if (body is CompoundStmt compoundStmt)
                {
                    _outputBuilder.BeginConstructorInitializers();
                    VisitStmts(compoundStmt.Body);
                    _outputBuilder.EndConstructorInitializers();
                }
                else
                {
                    _outputBuilder.BeginInnerFunctionBody();
                    Visit(body);
                    _outputBuilder.EndInnerFunctionBody();
                }

                _outputBuilder.EndBody();
                _outputBuilder.EndFunctionOrDelegate(isVirtual, false);
            }

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

                    var memberRef = (Ref)cxxConstructorDecl.CursorChildren[i];
                    var memberInit = (Stmt)cxxConstructorDecl.CursorChildren[++i];

                    if (memberInit is ImplicitValueInitExpr)
                    {
                        continue;
                    }

                    var memberRefName = GetRemappedCursorName(memberRef.Referenced);
                    var memberInitName = memberInit.Spelling;

                    if (memberInit is CastExpr {SubExprAsWritten: DeclRefExpr declRefExpr})
                    {
                        memberInitName = GetRemappedCursorName(declRefExpr.Decl);
                    }

                    _outputBuilder.BeginConstructorInitializer(memberRefName, memberInitName);

                    var memberRefTypeName = GetRemappedTypeName(memberRef, context: null, memberRef.Type, out var memberRefNativeTypeName);

                    UncheckStmt(memberRefTypeName, memberInit);

                    _outputBuilder.EndConstructorInitializer();
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

            var rootRecordDecl = anonymousRecordDecl;

            var contextNameParts = new Stack<string>();
            var contextTypeParts = new Stack<string>();

            while (rootRecordDecl.IsAnonymousStructOrUnion && (rootRecordDecl.Parent is RecordDecl parentRecordDecl))
            {
                var contextNamePart = GetRemappedCursorName(rootRecordDecl);

                if (contextNamePart.StartsWith("_"))
                {
                    var suffixLength = 0;

                    if (contextNamePart.EndsWith("_e__Union"))
                    {
                        suffixLength = 10;
                    }
                    else if (contextNamePart.EndsWith("_e__Struct"))
                    {
                        suffixLength = 11;
                    }

                    if (suffixLength != 0)
                    {
                        contextNamePart = contextNamePart.Substring(1, contextNamePart.Length - suffixLength);
                    }
                }

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

            var accessSpecifier = GetAccessSpecifier(anonymousRecordDecl);

            var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out _);
            var name = GetRemappedCursorName(fieldDecl);
            var escapedName = EscapeName(name);

            var rootRecordDeclName = GetRemappedCursorName(rootRecordDecl);

            if (_config.ExcludedNames.Contains($"{rootRecordDeclName}.{name}") || _config.ExcludedNames.Contains($"{rootRecordDeclName}::{name}"))
            {
                return;
            }

            var desc = new FieldDesc {
                AccessSpecifier = accessSpecifier,
                NativeTypeName = null,
                EscapedName = escapedName,
                Offset = null,
                NeedsNewKeyword = false,
                Location = fieldDecl.Location
            };

            _outputBuilder.WriteDivider(true);
            _outputBuilder.BeginField(in desc);

            var isFixedSizedBuffer = type.CanonicalType is ConstantArrayType;
            var generateCompatibleCode = _config.GenerateCompatibleCode;
            var typeString = string.Empty;

            if (!fieldDecl.IsBitField && (!isFixedSizedBuffer || generateCompatibleCode))
            {
                typeString = "ref ";
            }

            if (type.CanonicalType is RecordType recordType)
            {
                var recordDecl = recordType.Decl;

                while ((recordDecl.DeclContext is RecordDecl parentRecordDecl) && (parentRecordDecl != rootRecordDecl))
                {
                    var parentRecordDeclName = GetRemappedCursorName(parentRecordDecl);
                    var escapedParentRecordDeclName = EscapeName(parentRecordDeclName);

                    typeString += escapedParentRecordDeclName + '.';

                    recordDecl = parentRecordDecl;
                }
            }

            var isSupportedFixedSizedBufferType =
                isFixedSizedBuffer && IsSupportedFixedSizedBufferType(typeName);

            if (isFixedSizedBuffer)
            {
                if (!generateCompatibleCode)
                {
                    _outputBuilder.EmitSystemSupport();
                    typeString += "Span<";
                }
                else if (!isSupportedFixedSizedBufferType)
                {
                    typeString += contextType + '.';
                    typeName = GetArtificialFixedSizedBufferName(fieldDecl);
                }
            }

            typeString += typeName;
            if (isFixedSizedBuffer && !generateCompatibleCode)
            {
                typeString += '>';
            }

            _outputBuilder.WriteRegularField(typeString, escapedName);

            var isIndirectPointerField = ((type.CanonicalType is PointerType) || (type.CanonicalType is ReferenceType)) && (typeName != "IntPtr") && (typeName != "UIntPtr");

            _outputBuilder.BeginBody();
            _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining);
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
                    if (isSupportedFixedSizedBufferType)
                    {
                        code.Write("[0], ");
                        code.Write(((ConstantArrayType)type.CanonicalType).Size);
                    }
                    else
                    {
                        code.Write(".AsSpan(");
                    }
                }
                else
                {
                    code.Write(", 1)");
                }

                code.Write(')');

                if (isIndirectPointerField)
                {
                    code.Write('.');
                    code.Write(contextName);
                    code.Write('.');
                    code.Write(escapedName);
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);

                _outputBuilder.EndGetter();
            }

            _outputBuilder.EndBody();
            _outputBuilder.EndField(false);
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
                AddDiagnostic(DiagnosticLevel.Error,
                    $"Unsupported parameter variable declaration parent: '{previousContext.CursorKindSpelling}'. Generated bindings may be incomplete.",
                    previousContext);
            }

            void ForFunctionDecl(ParmVarDecl parmVarDecl, FunctionDecl functionDecl)
            {
                var type = parmVarDecl.Type;
                var typeName = GetTargetTypeName(parmVarDecl, out var nativeTypeName);

                var name = GetRemappedCursorName(parmVarDecl);
                var escapedName = EscapeName(name);

                var parameters = functionDecl.Parameters;
                var index = parameters.IndexOf(parmVarDecl);
                var lastIndex = parameters.Count - 1;

                if (name.Equals("param"))
                {
                    escapedName += index;
                }

                var desc = new ParameterDesc<(PInvokeGenerator This, CSharp.CSharpOutputBuilder CSharpOutputBuilder, Expr DefaultArg)>
                {
                    Name = escapedName,
                    Type = typeName,
                    NativeTypeName = nativeTypeName,
                    CppAttributes = _config.GenerateCppAttributes
                        ? parmVarDecl.Attrs.Select(x => EscapeString(x.Spelling))
                        : null,
                    Location = parmVarDecl.Location
                };

                var handledDefaultArg = false;
                var isExprDefaultValue = false;

                if (parmVarDecl.HasDefaultArg)
                {
                    isExprDefaultValue = IsDefaultValue(parmVarDecl.DefaultArg);

                    if ((_outputBuilder is CSharp.CSharpOutputBuilder csharpOutputBuilder) && (_config.WithTransparentStructs.ContainsKey(typeName) || parameters.Skip(index).Any((parmVarDecl) => {
                        var type = parmVarDecl.Type;
                        var typeName = GetTargetTypeName(parmVarDecl, out var nativeTypeName);
                        return _config.WithTransparentStructs.ContainsKey(typeName);
                    })))
                    {
                        desc.CustomAttrGeneratorData = (this, csharpOutputBuilder, isExprDefaultValue ? null : parmVarDecl.DefaultArg);
                        desc.WriteCustomAttrs = static x => {
                            if (x.DefaultArg is not null)
                            {
                                x.CSharpOutputBuilder.WriteCustomAttribute("Optional, DefaultParameterValue(", () => {
                                    x.This.Visit(x.DefaultArg);
                                    x.CSharpOutputBuilder.Write(')');
                                });
                            }
                            else
                            {
                                x.CSharpOutputBuilder.WriteCustomAttribute("Optional", null);
                            }
                        };
                        handledDefaultArg = true;
                    }
                }

                _outputBuilder.BeginParameter(in desc);

                if (parmVarDecl.HasDefaultArg && !handledDefaultArg)
                {
                    _outputBuilder.BeginParameterDefault();

                    var defaultArg = parmVarDecl.DefaultArg;

                    if (parmVarDecl.Type.CanonicalType.IsPointerType && (defaultArg.Handle.Evaluate.Kind == CXEvalResultKind.CXEval_UnExposed))
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
                        Visit(parmVarDecl.DefaultArg);
                    }

                    _outputBuilder.EndParameterDefault();
                }

                _outputBuilder.EndParameter();

                if (index != lastIndex)
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

                if (name.Equals("param"))
                {
                    escapedName += index;
                }

                var desc = new ParameterDesc<(string Name, PInvokeGenerator This)>
                {
                    Name = escapedName,
                    Type = typeName,
                    NativeTypeName = nativeTypeName,
                    CppAttributes = _config.GenerateCppAttributes
                        ? parmVarDecl.Attrs.Select(x => EscapeString(x.Spelling))
                        : null,
                    Location = parmVarDecl.Location
                };

                _outputBuilder.BeginParameter(in desc);

                if (parmVarDecl.HasDefaultArg)
                {
                    _outputBuilder.BeginParameterDefault();
                    Visit(parmVarDecl.DefaultArg);
                    _outputBuilder.EndParameterDefault();
                }

                _outputBuilder.EndParameter();

                if (index != lastIndex)
                {
                    _outputBuilder.WriteParameterSeparator();
                }
            }

            bool IsDefaultValue(Expr defaultArg)
            {
                return IsStmtAsWritten<CXXNullPtrLiteralExpr>(defaultArg, out _, removeParens: true) ||
                       (IsStmtAsWritten<CastExpr>(defaultArg, out var castExpr, removeParens: true) && (castExpr.CastKind == CX_CastKind.CX_CK_NullToPointer)) ||
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
                var hasVtbl = false;

                if (cxxRecordDecl != null)
                {
                    hasVtbl = HasVtbl(cxxRecordDecl);
                }

                var alignment = Math.Max(recordDecl.TypeForDecl.Handle.AlignOf, 1);
                var maxAlignm = recordDecl.Fields.Any() ? recordDecl.Fields.Max((fieldDecl) => Math.Max(fieldDecl.Type.Handle.AlignOf, 1)) : alignment;

                var generateTestsClass = _testOutputBuilder != null && !recordDecl.IsAnonymousStructOrUnion && recordDecl.DeclContext is not RecordDecl;

                if (generateTestsClass)
                {
                    _testOutputBuilder.WriteIndented("/// <summary>Provides validation of the <see cref=\"");
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.WriteLine("\" /> struct.</summary>");

                    WithAttributes("*", onlySupportedOSPlatform: true, isTestOutput: true);
                    WithAttributes(name, onlySupportedOSPlatform: true, isTestOutput: true);

                    _testOutputBuilder.WriteIndented("public static unsafe partial class ");
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.WriteLine("Tests");
                    _testOutputBuilder.WriteBlockStart();
                }

                Guid? nullableUuid = null;
                if (TryGetUuid(recordDecl, out var uuid))
                {
                    nullableUuid = uuid;
                    var iidName = GetRemappedName($"IID_{nativeName}", recordDecl, tryRemapOperatorName: false, out var wasRemapped, skipUsing: true);

                    _uuidsToGenerate.Add(iidName, uuid);

                    if (_testOutputBuilder != null)
                    {
                        var className = GetClass(iidName);

                        _testOutputBuilder.AddUsingDirective("System");
                        _testOutputBuilder.AddUsingDirective($"static {GetNamespace(className)}.{className}");

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

                        _testOutputBuilder.Write(iidName);

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

                var layoutKind = recordDecl.IsUnion
                    ? LayoutKind.Explicit
                    : LayoutKind.Sequential;

                long alignment32 = -1;
                long alignment64 = -1;

                GetTypeSize(recordDecl, recordDecl.TypeForDecl, ref alignment32, ref alignment64, out var size32, out var size64);

                string[] baseTypeNames = null;

                string nativeNameWithExtras = null, nativeInheritance = null;
                if ((cxxRecordDecl != null) && cxxRecordDecl.Bases.Any())
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
                    nativeInheritance = GetCursorName(cxxRecordDecl.Bases.Last().Referenced);
                    baseTypeNames = baseTypeNamesBuilder.ToArray();
                }

                var desc = new StructDesc<(string Name, PInvokeGenerator This)>
                {
                    AccessSpecifier = GetAccessSpecifier(recordDecl),
                    EscapedName = escapedName,
                    IsUnsafe = IsUnsafe(recordDecl),
                    HasVtbl = hasVtbl,
                    IsUnion = recordDecl.IsUnion,
                    Layout = new() {
                        Alignment32 = alignment32,
                        Alignment64 = alignment64,
                        Size32 = size32,
                        Size64 = size64,
                        Pack = alignment < maxAlignm ? alignment : 0,
                        MaxFieldAlignment = maxAlignm,
                        Kind = layoutKind
                    },
                    Uuid = nullableUuid,
                    NativeType = nativeNameWithExtras,
                    NativeInheritance = _config.GenerateNativeInheritanceAttribute ? nativeInheritance : null,
                    Location = recordDecl.Location,
                    IsNested = recordDecl.DeclContext is TagDecl,
                };
                _outputBuilder.BeginStruct(in desc);

                if (hasVtbl)
                {
                    var fieldDesc = new FieldDesc
                    {
                        AccessSpecifier = AccessSpecifier.Public,
                        NativeTypeName = null,
                        EscapedName = "lpVtbl",
                        Offset = null,
                        NeedsNewKeyword = false
                    };

                    _outputBuilder.BeginField(in fieldDesc);

                    if (_config.GenerateExplicitVtbls)
                    {
                        _outputBuilder.WriteRegularField("Vtbl*", "lpVtbl");
                    }
                    else
                    {
                        _outputBuilder.WriteRegularField("void**", "lpVtbl");
                    }

                    _outputBuilder.EndField();
                }

                if (cxxRecordDecl != null)
                {
                    foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                    {
                        var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                        if (HasFields(baseCxxRecordDecl))
                        {
                            var parent = GetRemappedCursorName(baseCxxRecordDecl);
                            var baseFieldName = GetAnonymousName(cxxBaseSpecifier, "Base");
                            baseFieldName = GetRemappedName(baseFieldName, cxxBaseSpecifier, tryRemapOperatorName: true, out var wasRemapped, skipUsing: true);

                            var fieldDesc = new FieldDesc
                            {
                                AccessSpecifier = GetAccessSpecifier(baseCxxRecordDecl),
                                NativeTypeName = null,
                                EscapedName = baseFieldName,
                                Offset = null,
                                NeedsNewKeyword = false,
                                InheritedFrom = parent,
                                Location = cxxBaseSpecifier.Location
                            };

                            _outputBuilder.BeginField(in fieldDesc);
                            _outputBuilder.WriteRegularField(parent, baseFieldName);
                            _outputBuilder.EndField();
                        }
                    }
                }

                if (generateTestsClass)
                {
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

                var bitfieldTypes = GetBitfieldCount(recordDecl);
                var bitfieldIndex = (bitfieldTypes.Length == 1) ? -1 : 0;

                var bitfieldPreviousSize = 0L;
                var bitfieldRemainingBits = 0L;

                foreach (var fieldDecl in recordDecl.Fields)
                {
                    if (fieldDecl.IsBitField)
                    {
                        VisitBitfieldDecl(fieldDecl, bitfieldTypes, recordDecl, contextName: "", ref bitfieldIndex, ref bitfieldPreviousSize, ref bitfieldRemainingBits);
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

                if (cxxRecordDecl != null)
                {
                    foreach (var cxxConstructorDecl in cxxRecordDecl.Ctors)
                    {
                        Visit(cxxConstructorDecl);
                        _outputBuilder.WriteDivider();
                    }

                    if (cxxRecordDecl.HasUserDeclaredDestructor && !cxxRecordDecl.Destructor.IsVirtual)
                    {
                        Visit(cxxRecordDecl.Destructor);
                        _outputBuilder.WriteDivider();
                    }

                    if (hasVtbl)
                    {
                        OutputDelegateSignatures(cxxRecordDecl, cxxRecordDecl);
                    }
                }

                var excludedCursors = recordDecl.Fields.AsEnumerable<Cursor>().Concat(recordDecl.IndirectFields);

                if (cxxRecordDecl != null)
                {
                    OutputMethods(cxxRecordDecl, cxxRecordDecl);
                    excludedCursors = excludedCursors.Concat(cxxRecordDecl.Methods);
                }

                Visit(recordDecl.Decls, excludedCursors);

                foreach (var constantArray in recordDecl.Fields.Where((field) => field.Type.CanonicalType is ConstantArrayType))
                {
                    VisitConstantArrayFieldDecl(recordDecl, constantArray);
                }

                if (hasVtbl)
                {
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
                        if (_outputBuilder is CSharp.CSharpOutputBuilder csharpOutputBuilder)
                        {
                            csharpOutputBuilder.NeedsNewline = true;
                        }

                        _outputBuilder.BeginMarkerInterface(baseTypeNames);
                        OutputMarkerInterfaces(cxxRecordDecl, cxxRecordDecl);
                        _outputBuilder.EndMarkerInterface();
                    }

                    if (_config.GenerateExplicitVtbls || _config.GenerateTrimmableVtbls)
                    {
                        if (_outputBuilder is CSharp.CSharpOutputBuilder csharpOutputBuilder)
                        {
                            csharpOutputBuilder.NeedsNewline = true;
                        }

                        _outputBuilder.BeginExplicitVtbl();
                        OutputVtblEntries(cxxRecordDecl, cxxRecordDecl);
                        _outputBuilder.EndExplicitVtbl();
                    }
                }

                _outputBuilder.EndStruct();

                if (generateTestsClass)
                {
                    _testOutputBuilder.WriteBlockEnd();
                }
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

            bool HasFields(RecordDecl recordDecl)
            {
                if (recordDecl.Fields.Count != 0)
                {
                    return true;
                }

                foreach (var decl in recordDecl.Decls)
                {
                    if ((decl is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion &&
                        HasFields(nestedRecordDecl))
                    {
                        return true;
                    }
                }

                if (recordDecl is CXXRecordDecl cxxRecordDecl)
                {
                    foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                    {
                        var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                        if (HasFields(baseCxxRecordDecl))
                        {
                            return true;
                        }
                    }
                }

                return false;
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

                if (_config.GenerateTrimmableVtbls && cxxMethodDecl.Parameters.Any((parmVarDecl) => (parmVarDecl.Type.CanonicalType is PointerType pointerType) && (pointerType.PointeeType is FunctionType)))
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

                if (returnType.Kind != CXTypeKind.CXType_Void)
                {
                    needsReturnFixup = NeedsReturnFixup(cxxMethodDecl);
                    needsCastToTransparentStruct = _config.WithTransparentStructs.TryGetValue(returnTypeName, out var transparentStruct) && IsTransparentStructHandle(transparentStruct.Kind);
                }

                var desc = new FunctionOrDelegateDesc<(string Name, PInvokeGenerator This)> {
                    AccessSpecifier = AccessSpecifier.Public,
                    EscapedName = EscapeAndStripName(name),
                    IsMemberFunction = true,
                    NativeTypeName = nativeTypeName,
                    HasFnPtrCodeGen = !_config.ExcludeFnptrCodegen,
                    IsCtxCxxRecord = true,
                    IsCxxRecordCtxUnsafe = IsUnsafe(cxxRecordDecl),
                    IsUnsafe = true,
                    NeedsReturnFixup = needsReturnFixup,
                    ReturnType = returnTypeName,
                    VtblIndex = _config.GenerateVtblIndexAttribute ? cxxMethodDecl.VtblIndex : -1,
                    Location = cxxMethodDecl.Location
                };

                var isUnsafe = true;
                _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);

                _outputBuilder.BeginFunctionInnerPrototype(desc.EscapedName);

                Visit(cxxMethodDecl.Parameters);

                _outputBuilder.EndFunctionInnerPrototype();
                _outputBuilder.EndFunctionOrDelegate(false, isBodyless: true);

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

                var remappedName = FixupNameForMultipleHits(cxxMethodDecl);
                var escapedName = EscapeAndStripName(remappedName);

                var desc = new FieldDesc
                {
                    AccessSpecifier = AccessSpecifier.Public,
                    NativeTypeName = nativeTypeName,
                    EscapedName = escapedName,
                    Offset = null,
                    NeedsNewKeyword = NeedsNewKeyword(remappedName),
                    Location = cxxMethodDecl.Location
                };

                _outputBuilder.BeginField(in desc);
                _outputBuilder.WriteRegularField(cxxMethodDeclTypeName, escapedName);
                _outputBuilder.EndField();

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

                if (returnType.Kind != CXTypeKind.CXType_Void)
                {
                    needsReturnFixup = NeedsReturnFixup(cxxMethodDecl);
                    needsCastToTransparentStruct = _config.WithTransparentStructs.TryGetValue(returnTypeName, out var transparentStruct) && IsTransparentStructHandle(transparentStruct.Kind);
                }

                var desc = new FunctionOrDelegateDesc<(string Name, PInvokeGenerator This)>
                {
                    AccessSpecifier = AccessSpecifier.Public,
                    IsAggressivelyInlined = _config.GenerateAggressiveInlining,
                    EscapedName = EscapeAndStripName(name),
                    IsMemberFunction = true,
                    NativeTypeName = nativeTypeName,
                    NeedsNewKeyword = NeedsNewKeyword(name, cxxMethodDecl.Parameters),
                    HasFnPtrCodeGen = !_config.ExcludeFnptrCodegen,
                    IsCtxCxxRecord = true,
                    IsCxxRecordCtxUnsafe = IsUnsafe(cxxRecordDecl),
                    IsUnsafe = true,
                    NeedsReturnFixup = needsReturnFixup,
                    ReturnType = returnTypeName,
                    VtblIndex = _config.GenerateVtblIndexAttribute ? cxxMethodDecl.VtblIndex : -1,
                    Location = cxxMethodDecl.Location
                };

                var isUnsafe = true;
                _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);

                _outputBuilder.BeginFunctionInnerPrototype(desc.EscapedName);

                Visit(cxxMethodDecl.Parameters);

                _outputBuilder.EndFunctionInnerPrototype();
                _outputBuilder.BeginBody();

                var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
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

                if (returnType.Kind != CXTypeKind.CXType_Void)
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
                    body.Write(PrefixAndStripName(name, GetOverloadIndex(cxxMethodDecl)));
                    body.EndMarker("delegate");
                    body.Write(">(");
                }

                if (_config.GenerateExplicitVtbls)
                {
                    body.Write("lpVtbl->");
                    body.BeginMarker("vtbl", new KeyValuePair<string, object>("explicit", true));
                    body.Write(EscapeAndStripName(remappedName));
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
                    if (parmVarDeclName.Equals("param"))
                    {
                        escapedParmVarDeclName += index;
                    }

                    body.BeginMarker("param", new KeyValuePair<string, object>("name", escapedParmVarDeclName));
                    body.Write(escapedParmVarDeclName);
                    body.EndMarker("param");
                }

                body.Write(')');

                if (returnTypeName == "bool")
                {
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
                _outputBuilder.EndFunctionOrDelegate(false, false);

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

            void VisitBitfieldDecl(FieldDecl fieldDecl, Type[] types, RecordDecl recordDecl, string contextName, ref int index, ref long previousSize, ref long remainingBits)
            {
                Debug.Assert(fieldDecl.IsBitField);

                var type = fieldDecl.Type;
                var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out var nativeTypeName);

                if (string.IsNullOrWhiteSpace(nativeTypeName))
                {
                    nativeTypeName = typeName;
                }

                nativeTypeName += $" : {fieldDecl.BitWidthValue}";

                var currentSize = fieldDecl.Type.Handle.SizeOf;

                var bitfieldName = "_bitfield";

                Type typeBacking;
                string typeNameBacking;

                if ((!_config.GenerateUnixTypes && (currentSize != previousSize)) || (fieldDecl.BitWidthValue > remainingBits))
                {
                    if (index >= 0)
                    {
                        index++;
                        bitfieldName += index.ToString();
                    }

                    remainingBits = currentSize * 8;
                    previousSize = 0;

                    typeBacking = (index > 0) ? types[index - 1] : types[0];
                    typeNameBacking = GetRemappedTypeName(fieldDecl, context: null, typeBacking, out _);

                    if (fieldDecl.Parent == recordDecl)
                    {
                        var fieldDesc = new FieldDesc
                        {
                            AccessSpecifier = AccessSpecifier.Public,
                            NativeTypeName = null,
                            EscapedName = bitfieldName,
                            Offset = fieldDecl.Parent.IsUnion ? 0 : null,
                            NeedsNewKeyword = false,
                            Location = fieldDecl.Location
                        };
                        _outputBuilder.BeginField(in fieldDesc);
                        _outputBuilder.WriteRegularField(typeNameBacking, bitfieldName);
                        _outputBuilder.EndField();
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
                        bitfieldName += index.ToString();
                    }

                    typeBacking = (index > 0) ? types[index - 1] : types[0];
                    typeNameBacking = GetRemappedTypeName(fieldDecl, context: null, typeBacking, out _);
                }

                var bitfieldOffset = (currentSize * 8) - remainingBits;

                var bitwidthHexStringBacking = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X");
                var canonicalTypeBacking = typeBacking.CanonicalType;

                switch (canonicalTypeBacking.Kind)
                {
                    case CXTypeKind.CXType_Char_U:
                    case CXTypeKind.CXType_UChar:
                    case CXTypeKind.CXType_UShort:
                    case CXTypeKind.CXType_UInt:
                    {
                        bitwidthHexStringBacking += "u";
                        break;
                    }

                    case CXTypeKind.CXType_ULong:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto default;
                        }

                        goto case CXTypeKind.CXType_UInt;
                    }

                    case CXTypeKind.CXType_ULongLong:
                    {
                        if (typeNameBacking == "nuint")
                        {
                            goto case CXTypeKind.CXType_UInt;
                        }

                        bitwidthHexStringBacking += "UL";
                        break;
                    }

                    case CXTypeKind.CXType_Char_S:
                    case CXTypeKind.CXType_SChar:
                    case CXTypeKind.CXType_Short:
                    case CXTypeKind.CXType_Int:
                    {
                        break;
                    }

                    case CXTypeKind.CXType_Long:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto default;
                        }

                        goto case CXTypeKind.CXType_Int;
                    }

                    case CXTypeKind.CXType_LongLong:
                    {
                        if (typeNameBacking == "nint")
                        {
                            goto case CXTypeKind.CXType_Int;
                        }

                        bitwidthHexStringBacking += "L";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{canonicalTypeBacking.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                        break;
                    }
                }

                var bitwidthHexString = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X");

                var canonicalType = type.CanonicalType;

                if (canonicalType is EnumType enumType)
                {
                    canonicalType = enumType.Decl.IntegerType.CanonicalType;
                }

                switch (canonicalType.Kind)
                {
                    case CXTypeKind.CXType_Char_U:
                    case CXTypeKind.CXType_UChar:
                    case CXTypeKind.CXType_UShort:
                    case CXTypeKind.CXType_UInt:
                    {
                        bitwidthHexString += "u";
                        break;
                    }

                    case CXTypeKind.CXType_ULong:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto default;
                        }

                        goto case CXTypeKind.CXType_UInt;
                    }

                    case CXTypeKind.CXType_ULongLong:
                    {
                        if (typeNameBacking == "nuint")
                        {
                            goto case CXTypeKind.CXType_UInt;
                        }

                        bitwidthHexString += "UL";
                        break;
                    }

                    case CXTypeKind.CXType_Char_S:
                    case CXTypeKind.CXType_SChar:
                    case CXTypeKind.CXType_Short:
                    case CXTypeKind.CXType_Int:
                    {
                        break;
                    }

                    case CXTypeKind.CXType_Long:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto default;
                        }

                        goto case CXTypeKind.CXType_Int;
                    }

                    case CXTypeKind.CXType_LongLong:
                    {
                        if (typeNameBacking == "nint")
                        {
                            goto case CXTypeKind.CXType_Int;
                        }

                        bitwidthHexString += "L";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{canonicalType.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                        break;
                    }
                }

                canonicalType = type.CanonicalType;

                var accessSpecifier = GetAccessSpecifier(fieldDecl);
                var name = GetRemappedCursorName(fieldDecl);
                var escapedName = EscapeName(name);

                var desc = new FieldDesc
                {
                    AccessSpecifier = accessSpecifier,
                    NativeTypeName = nativeTypeName,
                    EscapedName = escapedName,
                    Offset = null,
                    NeedsNewKeyword = false,
                    Location = fieldDecl.Location
                };

                _outputBuilder.WriteDivider();
                _outputBuilder.BeginField(in desc);
                _outputBuilder.WriteRegularField(typeName, escapedName);
                _outputBuilder.BeginBody();
                _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining);
                var code = _outputBuilder.BeginCSharpCode();

                code.WriteIndented("return ");

                var recordDeclName = GetCursorName(recordDecl);

                var isRemappedToSelf = _config.RemappedNames.TryGetValue(typeName, out var remappedTypeName) && typeName.Equals(remappedTypeName);
                var needsCast = (currentSize < 4) || (canonicalTypeBacking != canonicalType) || isRemappedToSelf;

                if (needsCast)
                {
                    code.Write('(');
                    code.BeginMarker("typeName");
                    code.Write(typeName);
                    code.EndMarker("typeName");
                    code.Write(")(");
                }

                if (bitfieldOffset != 0)
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

                if (bitfieldOffset != 0)
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

                if (needsCast)
                {
                    code.Write(')');
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);
                _outputBuilder.EndGetter();

                _outputBuilder.BeginSetter(_config.GenerateAggressiveInlining);
                code = _outputBuilder.BeginCSharpCode();
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

                if (currentSize < 4)
                {
                    code.Write('(');
                    code.BeginMarker("typeNameBacking");
                    code.Write(typeNameBacking);
                    code.EndMarker("typeNameBacking");
                    code.Write(")(");
                }

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

                code.Write(") | ");

                if ((canonicalTypeBacking != canonicalType) && (canonicalType is not EnumType))
                {
                    code.Write('(');
                    code.Write(typeNameBacking);
                    code.Write(')');
                }

                code.Write('(');

                if (bitfieldOffset != 0)
                {
                    code.Write('(');
                }

                if ((canonicalType is EnumType) || isRemappedToSelf)
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

                if (currentSize < 4)
                {
                    code.Write(')');
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);
                _outputBuilder.EndSetter();
                _outputBuilder.EndBody();
                _outputBuilder.EndField(false);
                _outputBuilder.WriteDivider();

                remainingBits -= fieldDecl.BitWidthValue;
                previousSize = Math.Max(previousSize, currentSize);
            }

            void VisitConstantArrayFieldDecl(RecordDecl recordDecl, FieldDecl constantArray)
            {
                Debug.Assert(constantArray.Type.CanonicalType is ConstantArrayType);

                var outputBuilder = _outputBuilder;
                var type = (ConstantArrayType)constantArray.Type.CanonicalType;
                var typeName = GetRemappedTypeName(constantArray, context: null, constantArray.Type, out _);

                if (IsSupportedFixedSizedBufferType(typeName))
                {
                    return;
                }

                _outputBuilder.WriteDivider();

                var alignment = Math.Max(recordDecl.TypeForDecl.Handle.AlignOf, 1);
                var maxAlignm = recordDecl.Fields.Any() ? recordDecl.Fields.Max((fieldDecl) => Math.Max(fieldDecl.Type.Handle.AlignOf, 1)) : alignment;

                var accessSpecifier = GetAccessSpecifier(constantArray);
                var canonicalElementType = type.ElementType.CanonicalType;
                var isUnsafeElementType =
                    ((canonicalElementType is PointerType) || (canonicalElementType is ReferenceType)) &&
                    (typeName != "IntPtr") && (typeName != "UIntPtr");

                var name = GetArtificialFixedSizedBufferName(constantArray);
                var escapedName = EscapeName(name);

                var totalSize = Math.Max(type.Size, 1);
                var sizePerDimension = new List<(long index, long size)>() {(0, type.Size)};

                var elementType = type.ElementType;

                while (elementType.CanonicalType is ConstantArrayType subConstantArrayType)
                {
                    totalSize *= Math.Max(subConstantArrayType.Size, 1);
                    sizePerDimension.Add((0, Math.Max(subConstantArrayType.Size, 1)));
                    elementType = subConstantArrayType.ElementType;
                }

                long alignment32 = -1;
                long alignment64 = -1;

                GetTypeSize(constantArray, constantArray.Type, ref alignment32, ref alignment64, out var size32, out var size64);

                if ((size32 == 0 || size64 == 0) && _testOutputBuilder != null)
                {
                    AddDiagnostic(DiagnosticLevel.Info, $"{escapedName} (constant array field) has a size of 0", constantArray);
                }

                var desc = new StructDesc<(string Name, PInvokeGenerator This)> {
                    AccessSpecifier = accessSpecifier,
                    EscapedName = escapedName,
                    IsUnsafe = isUnsafeElementType,
                    Layout = new() {
                        Alignment32 = alignment32,
                        Alignment64 = alignment64,
                        Size32 = size32,
                        Size64 = size64,
                        Pack = alignment < maxAlignm ? alignment : 0,
                        MaxFieldAlignment = maxAlignm,
                        Kind = LayoutKind.Sequential
                    },
                    Location = constantArray.Location,
                    IsNested = true,
                };

                _outputBuilder.BeginStruct(in desc);

                var firstFieldName = "";

                for (long i = 0; i < totalSize; i++)
                {
                    var dimension = sizePerDimension[0];
                    var firstDimension = dimension.index++;
                    var fieldName = "e" + firstDimension;
                    sizePerDimension[0] = dimension;

                    var separateStride = false;
                    for (var d = 1; d < sizePerDimension.Count; d++)
                    {
                        dimension = sizePerDimension[d];
                        fieldName += "_" + dimension.index;
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

                    if (firstFieldName == "")
                    {
                        firstFieldName = fieldName;
                    }

                    var fieldDesc = new FieldDesc
                    {
                        AccessSpecifier = accessSpecifier,
                        NativeTypeName = null,
                        EscapedName = fieldName,
                        Offset = null,
                        NeedsNewKeyword = false,
                        Location = constantArray.Location
                    };

                    _outputBuilder.BeginField(in fieldDesc);
                    _outputBuilder.WriteRegularField(typeName, fieldName);
                    _outputBuilder.EndField();
                    if (!separateStride)
                    {
                        _outputBuilder.SuppressDivider();
                    }
                }

                var generateCompatibleCode = _config.GenerateCompatibleCode;

                if (generateCompatibleCode || isUnsafeElementType)
                {
                    _outputBuilder.BeginIndexer(AccessSpecifier.Public, generateCompatibleCode && !isUnsafeElementType);
                    _outputBuilder.WriteIndexer($"ref {typeName}");
                    _outputBuilder.BeginIndexerParameters();
                    var param = new ParameterDesc<(string Name, PInvokeGenerator This)>
                    {
                        Name = "index",
                        Type = "int",
                    };
                    _outputBuilder.BeginParameter(in param);
                    _outputBuilder.EndParameter();
                    _outputBuilder.EndIndexerParameters();
                    _outputBuilder.BeginBody();

                    _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining);
                    var code = _outputBuilder.BeginCSharpCode();

                    code.WriteIndented("fixed (");
                    code.Write(typeName);
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
                else
                {
                    _outputBuilder.BeginIndexer(AccessSpecifier.Public, false);
                    _outputBuilder.WriteIndexer($"ref {typeName}");
                    _outputBuilder.BeginIndexerParameters();
                    var param = new ParameterDesc<(string Name, PInvokeGenerator This)>
                    {
                        Name = "index",
                        Type = "int",
                    };
                    _outputBuilder.BeginParameter(in param);
                    _outputBuilder.EndParameter();
                    _outputBuilder.EndIndexerParameters();
                    _outputBuilder.BeginBody();

                    _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining);
                    var code = _outputBuilder.BeginCSharpCode();
                    code.AddUsingDirective("System");
                    code.AddUsingDirective("System.Runtime.InteropServices");

                    code.WriteIndented("return ref AsSpan(");

                    if (type.Size == 1)
                    {
                        code.Write("int.MaxValue");
                    }

                    code.Write(")[index]");
                    code.WriteSemicolon();
                    code.WriteNewline();
                    _outputBuilder.EndCSharpCode(code);

                    _outputBuilder.EndGetter();
                    _outputBuilder.EndBody();
                    _outputBuilder.EndIndexer();

                    var function = new FunctionOrDelegateDesc<(string Name, PInvokeGenerator This)>
                    {
                        AccessSpecifier = AccessSpecifier.Public,
                        EscapedName = "AsSpan",
                        IsAggressivelyInlined = _config.GenerateAggressiveInlining,
                        IsStatic = false,
                        IsMemberFunction = true,
                        ReturnType = $"Span<{typeName}>",
                        Location = constantArray.Location
                    };

                    var isUnsafe = false;
                    _outputBuilder.BeginFunctionOrDelegate(in function, ref isUnsafe);

                    _outputBuilder.BeginFunctionInnerPrototype("AsSpan");

                    if (type.Size == 1)
                    {
                        param = new ParameterDesc<(string Name, PInvokeGenerator This)>
                        {
                            Name = "length",
                            Type = "int",
                        };

                        _outputBuilder.BeginParameter(in param);
                        _outputBuilder.EndParameter();
                    }

                    _outputBuilder.EndFunctionInnerPrototype();
                    _outputBuilder.BeginBody(true);
                    code = _outputBuilder.BeginCSharpCode();

                    code.Write("MemoryMarshal.CreateSpan(ref ");
                    code.Write(firstFieldName);
                    code.Write(", ");

                    if (type.Size == 1)
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
                    _outputBuilder.EndFunctionOrDelegate(false, false);
                }

                _outputBuilder.EndStruct();
            }
        }

        private void VisitTranslationUnitDecl(TranslationUnitDecl translationUnitDecl)
        {
            Visit(translationUnitDecl.Decls);
            Visit(translationUnitDecl.CursorChildren, translationUnitDecl.Decls);
        }

        private void VisitTypeAliasDecl(TypeAliasDecl typeAliasDecl)
        {
            // Nothing to generate for type alias declarations
        }

        private void VisitTypedefDecl(TypedefDecl typedefDecl, bool onlyHandleRemappings)
        {
            ForUnderlyingType(typedefDecl, typedefDecl.UnderlyingType, onlyHandleRemappings);

            void ForFunctionProtoType(TypedefDecl typedefDecl, FunctionProtoType functionProtoType, Type parentType, bool onlyHandleRemappings)
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
                    var desc = new FunctionOrDelegateDesc<(string Name, PInvokeGenerator This)>
                    {
                        AccessSpecifier = GetAccessSpecifier(typedefDecl),
                        CallingConvention = callingConventionName,
                        EscapedName = escapedName,
                        IsVirtual = true, // such that it outputs as a delegate
                        IsUnsafe = IsUnsafe(typedefDecl, functionProtoType),
                        NativeTypeName = nativeTypeName,
                        ReturnType = returnTypeName,
                        Location = typedefDecl.Location
                    };

                    var isUnsafe = desc.IsUnsafe;
                    _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);

                    _outputBuilder.BeginFunctionInnerPrototype(escapedName);

                    Visit(typedefDecl.CursorChildren.OfType<ParmVarDecl>());

                    _outputBuilder.EndFunctionInnerPrototype();
                    _outputBuilder.EndFunctionOrDelegate(true, true);
                }
                StopUsingOutputBuilder();
            }

            void ForPointeeType(TypedefDecl typedefDecl, Type parentType, Type pointeeType, bool onlyHandleRemappings)
            {
                if (pointeeType is AttributedType attributedType)
                {
                    ForPointeeType(typedefDecl, attributedType, attributedType.ModifiedType, onlyHandleRemappings);
                }
                else if (pointeeType is ElaboratedType elaboratedType)
                {
                    ForPointeeType(typedefDecl, elaboratedType, elaboratedType.NamedType, onlyHandleRemappings);
                }
                else if (pointeeType is FunctionProtoType functionProtoType)
                {
                    ForFunctionProtoType(typedefDecl, functionProtoType, parentType, onlyHandleRemappings);
                }
                else if (pointeeType is PointerType pointerType)
                {
                    ForPointeeType(typedefDecl, pointerType, pointerType.PointeeType, onlyHandleRemappings);
                }
                else if (pointeeType is TypedefType typedefType)
                {
                    ForPointeeType(typedefDecl, typedefType, typedefType.Decl.UnderlyingType, onlyHandleRemappings);
                }
                else if (pointeeType is not ConstantArrayType and not BuiltinType and not TagType and not TemplateTypeParmType)
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported pointee type: '{pointeeType.TypeClassSpelling}'. Generating bindings may be incomplete.", typedefDecl);
                }
            }

            void ForUnderlyingType(TypedefDecl typedefDecl, Type underlyingType, bool onlyHandleRemappings)
            {
                if (underlyingType is ArrayType arrayType)
                {
                    // Nothing to do for array types
                }
                else if (underlyingType is AttributedType attributedType)
                {
                    ForUnderlyingType(typedefDecl, attributedType.ModifiedType, onlyHandleRemappings);
                }
                else if (underlyingType is BuiltinType builtinType)
                {
                    // Nothing to do for builtin types
                }
                else if (underlyingType is DependentNameType dependentNameType)
                {
                    // Nothing to do for dependent name types
                }
                else if (underlyingType is ElaboratedType elaboratedType)
                {
                    ForUnderlyingType(typedefDecl, elaboratedType.NamedType, onlyHandleRemappings);
                }
                else if (underlyingType is FunctionProtoType functionProtoType)
                {
                    ForFunctionProtoType(typedefDecl, functionProtoType, parentType: null, onlyHandleRemappings);
                }
                else if (underlyingType is PointerType pointerType)
                {
                    ForPointeeType(typedefDecl, parentType: null, pointerType.PointeeType, onlyHandleRemappings);
                }
                else if (underlyingType is ReferenceType referenceType)
                {
                    ForPointeeType(typedefDecl, parentType: null, referenceType.PointeeType, onlyHandleRemappings);
                }
                else if (underlyingType is TagType underlyingTagType)
                {
                    var underlyingName = GetCursorName(underlyingTagType.AsTagDecl);
                    var typedefName = GetCursorName(typedefDecl);

                    if (underlyingName != typedefName)
                    {
                        if (!_validNameRemappings.TryGetValue(underlyingName, out var remappings))
                        {
                            remappings = new HashSet<string>();
                            _validNameRemappings[underlyingName] = remappings;
                        }

                        _ = remappings.Add(typedefName);
                    }
                }
                else if (underlyingType is TemplateSpecializationType templateSpecializationType)
                {
                    if (templateSpecializationType.IsTypeAlias)
                    {
                        ForUnderlyingType(typedefDecl, templateSpecializationType.AliasedType, onlyHandleRemappings);
                    }
                    else
                    {
                        // Nothing to do for non-aliased template specialization types
                    }
                }
                else if (underlyingType is TemplateTypeParmType templateTypeParmType)
                {
                    // Nothing to do for template type parameter types
                }
                else if (underlyingType is TypedefType typedefType)
                {
                    ForUnderlyingType(typedefDecl, typedefType.Decl.UnderlyingType, onlyHandleRemappings);
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported underlying type: '{underlyingType.TypeClassSpelling}'. Generating bindings may be incomplete.", typedefDecl);
                }

                return;
            }

            string GetUndecoratedName(Type type)
            {
                return type is AttributedType attributedType
                    ? GetUndecoratedName(attributedType.ModifiedType)
                    : type is ElaboratedType elaboratedType ? GetUndecoratedName(elaboratedType.NamedType) : type.AsString;
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
            else if (IsPrevContextDecl<TranslationUnitDecl>(out _, out _) || IsPrevContextDecl<LinkageSpecDecl>(out _, out _) || IsPrevContextDecl<RecordDecl>(out _, out _))
            {
                if (!varDecl.HasInit)
                {
                    // Nothing to do if a top level const declaration doesn't have an initializer
                    return;
                }

                var type = varDecl.Type;
                var isMacroDefinitionRecord = false;

                var nativeName = GetCursorName(varDecl);
                if (nativeName.StartsWith("ClangSharpMacro_" + ""))
                {
                    type = varDecl.Init.Type;
                    nativeName = nativeName["ClangSharpMacro_".Length..];
                    isMacroDefinitionRecord = true;
                }

                var accessSpecifier = GetAccessSpecifier(varDecl);
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
                }

                var openedOutputBuilder = false;
                var className = GetClass(name);

                if (_outputBuilder is null)
                {
                    openedOutputBuilder = true;

                    if (IsUnsafe(varDecl, type) && (!varDecl.HasInit || !IsStmtAsWritten<StringLiteral>(varDecl.Init, out _, removeParens: true)))
                    {
                        _isTopLevelClassUnsafe[className] = true;
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
                    _ = nativeTypeNameBuilder.Append(macroValue);

                    nativeTypeName = nativeTypeNameBuilder.ToString();
                }

                var kind = ValueKind.Unknown;
                var flags = ValueFlags.None;

                if (varDecl.HasInit)
                {
                    flags |= ValueFlags.Initializer;
                }

                if (type.IsLocalConstQualified || isMacroDefinitionRecord || (type is ConstantArrayType))
                {
                    flags |= ValueFlags.Constant;
                }

                if (IsStmtAsWritten<StringLiteral>(varDecl.Init, out var stringLiteral, removeParens: true))
                {
                    kind = ValueKind.String;

                    switch (stringLiteral.Kind)
                    {
                        case CX_CharacterKind.CX_CLK_Ascii:
                        case CX_CharacterKind.CX_CLK_UTF8:
                        {
                            typeName = "ReadOnlySpan<byte>";
                            break;
                        }

                        case CX_CharacterKind.CX_CLK_Wide:
                        {
                            if (_config.GenerateUnixTypes)
                            {
                                goto default;
                            }

                            goto case CX_CharacterKind.CX_CLK_UTF16;
                        }

                        case CX_CharacterKind.CX_CLK_UTF16:
                        {
                            kind = ValueKind.Primitive;
                            typeName = "string";
                            break;
                        }

                        default:
                        {
                            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported string literal kind: '{stringLiteral.Kind}'. Generated bindings may be incomplete.", stringLiteral);
                            break;
                        }
                    }
                }
                else if (IsPrimitiveValue(type))
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
                else if ((varDecl.StorageClass == CX_StorageClass.CX_SC_Static) || openedOutputBuilder)
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

                    if (type is ArrayType arrayType)
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

                if (typeName == "Guid")
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
                    Location = varDecl.Location
                };

                if (openedOutputBuilder)
                {
                    StartUsingOutputBuilder(className);

                    if ((kind == ValueKind.String) && (typeName == "ReadOnlySpan<byte>"))
                    {
                        _outputBuilder.EmitSystemSupport();
                    }
                }

                WithAttributes("*");
                WithAttributes(name);

                WithUsings("*");
                WithUsings(name);

                _outputBuilder.BeginValue(in desc);
                _context.Last.Value = (_context.Last.Value.Cursor, desc);

                if (varDecl.HasInit)
                {
                    var dereference = (type.CanonicalType is PointerType pointerType) &&
                                      (pointerType.PointeeType.CanonicalType is FunctionType) &&
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
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported variable declaration parent: '{previousContext.CursorKindSpelling}'. Generated bindings may be incomplete.", previousContext);
            }

            void ForDeclStmt(VarDecl varDecl, DeclStmt declStmt)
            {
                var outputBuilder = StartCSharpCode();
                var name = GetRemappedCursorName(varDecl);
                var escapedName = EscapeName(name);

                if (varDecl == declStmt.Decls.First())
                {
                    var type = varDecl.Type;
                    var typeName = GetRemappedTypeName(varDecl, context: null, type, out _);

                    outputBuilder.Write(typeName);

                    if (type is ArrayType)
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

            bool IsPrimitiveValue(Type type)
            {
                if (type is AttributedType attributedType)
                {
                    return IsPrimitiveValue(attributedType.ModifiedType);
                }
                else if (type is AutoType autoType)
                {
                    return IsPrimitiveValue(autoType.CanonicalType);
                }
                else if (type is BuiltinType builtinType)
                {
                    switch (type.Kind)
                    {
                        case CXTypeKind.CXType_Bool:
                        case CXTypeKind.CXType_Char_U:
                        case CXTypeKind.CXType_UChar:
                        case CXTypeKind.CXType_Char16:
                        case CXTypeKind.CXType_UShort:
                        case CXTypeKind.CXType_UInt:
                        case CXTypeKind.CXType_ULong:
                        case CXTypeKind.CXType_ULongLong:
                        case CXTypeKind.CXType_Char_S:
                        case CXTypeKind.CXType_SChar:
                        case CXTypeKind.CXType_WChar:
                        case CXTypeKind.CXType_Short:
                        case CXTypeKind.CXType_Int:
                        case CXTypeKind.CXType_Long:
                        case CXTypeKind.CXType_LongLong:
                        case CXTypeKind.CXType_Float:
                        case CXTypeKind.CXType_Double:
                        {
                            return true;
                        }
                    }
                }
                else if (type is ElaboratedType elaboratedType)
                {
                    return IsPrimitiveValue(elaboratedType.NamedType);
                }
                else if (type is EnumType enumType)
                {
                    return IsPrimitiveValue(enumType.Decl.IntegerType);
                }
                else if (type is TypedefType typedefType)
                {
                    return IsPrimitiveValue(typedefType.Decl.UnderlyingType);
                }

                return type.IsPointerType;
            }
        }

        private bool IsConstant(string targetTypeName, Expr initExpr)
        {
            if (initExpr.Type.CanonicalType.IsPointerType && (targetTypeName != "string"))
            {
                return false;
            }

            switch (initExpr.StmtClass)
            {
                // case CX_StmtClass.CX_StmtClass_BinaryConditionalOperator:

                case CX_StmtClass.CX_StmtClass_ConditionalOperator:
                {
                    var conditionalOperator = (ConditionalOperator)initExpr;
                    return IsConstant(targetTypeName, conditionalOperator.Cond) && IsConstant(targetTypeName, conditionalOperator.LHS) && IsConstant(targetTypeName, conditionalOperator.RHS);
                }

                // case CX_StmtClass.CX_StmtClass_AddrLabelExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr:

                case CX_StmtClass.CX_StmtClass_ArraySubscriptExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_ArrayTypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_AsTypeExpr:
                // case CX_StmtClass.CX_StmtClass_AtomicExpr:

                case CX_StmtClass.CX_StmtClass_BinaryOperator:
                {
                    var binaryOperator = (BinaryOperator)initExpr;
                    return IsConstant(targetTypeName, binaryOperator.LHS) && IsConstant(targetTypeName, binaryOperator.RHS);
                }

                // case CX_StmtClass.CX_StmtClass_CompoundAssignOperator:
                // case CX_StmtClass.CX_StmtClass_BlockExpr:
                // case CX_StmtClass.CX_StmtClass_CXXBindTemporaryExpr:

                case CX_StmtClass.CX_StmtClass_CXXBoolLiteralExpr:
                {
                    return true;
                }

                case CX_StmtClass.CX_StmtClass_CXXConstructExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultArgExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDeleteExpr:

                case CX_StmtClass.CX_StmtClass_CXXDependentScopeMemberExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_CXXFoldExpr:
                // case CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr:

                case CX_StmtClass.CX_StmtClass_CXXNewExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_CXXNoexceptExpr:

                case CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr:
                {
                    return true;
                }

                // case CX_StmtClass.CX_StmtClass_CXXPseudoDestructorExpr:
                // case CX_StmtClass.CX_StmtClass_CXXRewrittenBinaryOperator:
                // case CX_StmtClass.CX_StmtClass_CXXScalarValueInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXStdInitializerListExpr:

                case CX_StmtClass.CX_StmtClass_CXXThisExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_CXXThrowExpr:
                // case CX_StmtClass.CX_StmtClass_CXXTypeidExpr:
                // case CX_StmtClass.CX_StmtClass_CXXUnresolvedConstructExpr:

                case CX_StmtClass.CX_StmtClass_CXXUuidofExpr:
                {
                    return false;
                }

                case CX_StmtClass.CX_StmtClass_CallExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_CUDAKernelCallExpr:
                // case CX_StmtClass.CX_StmtClass_CXXMemberCallExpr:

                case CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr:
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

                // case CX_StmtClass.CX_StmtClass_UserDefinedLiteral:
                // case CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr:

                case CX_StmtClass.CX_StmtClass_CStyleCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXStaticCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXFunctionalCastExpr:
                {
                    var cxxFunctionalCastExpr = (ExplicitCastExpr)initExpr;
                    return IsConstant(targetTypeName, cxxFunctionalCastExpr.SubExprAsWritten);
                }

                // case CX_StmtClass.CX_StmtClass_CXXConstCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr:

                case CX_StmtClass.CX_StmtClass_ImplicitCastExpr:
                {
                    var implicitCastExpr = (ImplicitCastExpr)initExpr;
                    return IsConstant(targetTypeName, implicitCastExpr.SubExprAsWritten);
                }

                case CX_StmtClass.CX_StmtClass_CharacterLiteral:
                {
                    return true;
                }

                // case CX_StmtClass.CX_StmtClass_ChooseExpr:
                // case CX_StmtClass.CX_StmtClass_CompoundLiteralExpr:
                // case CX_StmtClass.CX_StmtClass_ConceptSpecializationExpr:
                // case CX_StmtClass.CX_StmtClass_ConvertVectorExpr:
                // case CX_StmtClass.CX_StmtClass_CoawaitExpr:
                // case CX_StmtClass.CX_StmtClass_CoyieldExpr:

                case CX_StmtClass.CX_StmtClass_DeclRefExpr:
                {
                    var declRefExpr = (DeclRefExpr)initExpr;
                    return (declRefExpr.Decl is EnumConstantDecl) ||
                           ((declRefExpr.Decl is VarDecl varDecl) && varDecl.HasInit && IsConstant(targetTypeName, varDecl.Init));
                }

                // case CX_StmtClass.CX_StmtClass_DependentCoawaitExpr:
                // case CX_StmtClass.CX_StmtClass_DependentScopeDeclRefExpr:
                // case CX_StmtClass.CX_StmtClass_DesignatedInitExpr:
                // case CX_StmtClass.CX_StmtClass_DesignatedInitUpdateExpr:
                // case CX_StmtClass.CX_StmtClass_ExpressionTraitExpr:
                // case CX_StmtClass.CX_StmtClass_ExtVectorElementExpr:
                // case CX_StmtClass.CX_StmtClass_FixedPointLiteral:

                case CX_StmtClass.CX_StmtClass_FloatingLiteral:
                {
                    return true;
                }

                // case CX_StmtClass.CX_StmtClass_ConstantExpr:

                case CX_StmtClass.CX_StmtClass_ExprWithCleanups:
                {
                    var exprWithCleanups = (ExprWithCleanups)initExpr;
                    return IsConstant(targetTypeName, exprWithCleanups.SubExpr);
                }

                // case CX_StmtClass.CX_StmtClass_FunctionParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_GNUNullExpr:
                // case CX_StmtClass.CX_StmtClass_GenericSelectionExpr:
                // case CX_StmtClass.CX_StmtClass_ImaginaryLiteral:
                // case CX_StmtClass.CX_StmtClass_ImplicitValueInitExpr:

                case CX_StmtClass.CX_StmtClass_InitListExpr:
                {
                    return false;
                }

                case CX_StmtClass.CX_StmtClass_IntegerLiteral:
                {
                    return true;
                }

                // case CX_StmtClass.CX_StmtClass_LambdaExpr:
                // case CX_StmtClass.CX_StmtClass_MSPropertyRefExpr:
                // case CX_StmtClass.CX_StmtClass_MSPropertySubscriptExpr:
                // case CX_StmtClass.CX_StmtClass_MaterializeTemporaryExpr:

                case CX_StmtClass.CX_StmtClass_MemberExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_NoInitExpr:
                // case CX_StmtClass.CX_StmtClass_OMPArraySectionExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCArrayLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCAvailabilityCheckExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBoolLiteralExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBoxedExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCDictionaryLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCEncodeExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIndirectCopyRestoreExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIsaExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIvarRefExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCMessageExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCPropertyRefExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCProtocolExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCSelectorExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCStringLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCSubscriptRefExpr:

                case CX_StmtClass.CX_StmtClass_OffsetOfExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_OpaqueValueExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedLookupExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr:
                // case CX_StmtClass.CX_StmtClass_PackExpansionExpr:

                case CX_StmtClass.CX_StmtClass_ParenExpr:
                {
                    var parenExpr = (ParenExpr)initExpr;
                    return IsConstant(targetTypeName, parenExpr.SubExpr);
                }

                case CX_StmtClass.CX_StmtClass_ParenListExpr:
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

                // case CX_StmtClass.CX_StmtClass_PredefinedExpr:
                // case CX_StmtClass.CX_StmtClass_PseudoObjectExpr:
                // case CX_StmtClass.CX_StmtClass_RequiresExpr:
                // case CX_StmtClass.CX_StmtClass_ShuffleVectorExpr:
                // case CX_StmtClass.CX_StmtClass_SizeOfPackExpr:
                // case CX_StmtClass.CX_StmtClass_SourceLocExpr:
                // case CX_StmtClass.CX_StmtClass_StmtExpr:

                case CX_StmtClass.CX_StmtClass_StringLiteral:
                {
                    return true;
                }

                // case CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmExpr:
                // case CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_TypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_TypoExpr:

                case CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr:
                {
                    var unaryExprOrTypeTraitExpr = (UnaryExprOrTypeTraitExpr)initExpr;
                    var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;


                    long alignment32 = -1;
                    long alignment64 = -1;

                    GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out var size32, out var size64);

                    switch (unaryExprOrTypeTraitExpr.Kind)
                    {
                        case CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf:
                        {
                            return size32 == size64;
                        }

                        case CX_UnaryExprOrTypeTrait.CX_UETT_AlignOf:
                        case CX_UnaryExprOrTypeTrait.CX_UETT_PreferredAlignOf:
                        {
                            return alignment32 == alignment64;
                        }

                        default:
                        {
                            return false;
                        }
                    }
                }

                case CX_StmtClass.CX_StmtClass_UnaryOperator:
                {
                    var unaryOperator = (UnaryOperator)initExpr;

                    if (!IsConstant(targetTypeName, unaryOperator.SubExpr))
                    {
                        return false;
                    }

                    if (unaryOperator.Opcode != CX_UnaryOperatorKind.CX_UO_Minus)
                    {
                        return true;
                    }

                    return targetTypeName is not "IntPtr" and not "nint" and not "nuint" and not "UIntPtr";
                }

                // case CX_StmtClass.CX_StmtClass_VAArgExpr:

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported statement class: '{initExpr.StmtClassName}'. Generated bindings may not be constant.", initExpr);
                    return false;
                }
            }
        }
    }
}
