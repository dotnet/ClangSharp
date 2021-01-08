// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using ClangSharp.Interop;

namespace ClangSharp
{
    public partial class PInvokeGenerator
    {
        private void VisitClassTemplateDecl(ClassTemplateDecl classTemplateDecl)
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Class templates are not supported: '{GetCursorQualifiedName(classTemplateDecl)}'. Generated bindings may be incomplete.", classTemplateDecl);
        }

        private void VisitClassTemplateSpecializationDecl(ClassTemplateSpecializationDecl classTemplateSpecializationDecl)
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Class template specializations are not supported: '{GetCursorQualifiedName(classTemplateSpecializationDecl)}'. Generated bindings may be incomplete.", classTemplateSpecializationDecl);
        }

        private void VisitDecl(Decl decl)
        {
            if (IsExcluded(decl))
            {
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
                // case CX_DeclKind.CX_DeclKind_Friend:
                // case CX_DeclKind.CX_DeclKind_FriendTemplate:
                // case CX_DeclKind.CX_DeclKind_Import:

                case CX_DeclKind.CX_DeclKind_LinkageSpec:
                {
                    VisitLinkageSpecDecl((LinkageSpecDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_Label:

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
                // case CX_DeclKind.CX_DeclKind_TypeAlias:

                case CX_DeclKind.CX_DeclKind_Typedef:
                {
                    VisitTypedefDecl((TypedefDecl)decl);
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
                // case CX_DeclKind.CX_DeclKind_UsingShadow:
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

                // case CX_DeclKind.CX_DeclKind_IndirectField:
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
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported declaration: '{decl.Kind}'. Generated bindings may be incomplete.", decl);
                    break;
                }
            }
        }

        private void VisitEnumConstantDecl(EnumConstantDecl enumConstantDecl)
        {
            var accessSpecifier = string.Empty;
            var name = GetRemappedCursorName(enumConstantDecl);
            var escapedName = EscapeName(name);
            var isAnonymousEnum = false;

            var typeName = string.Empty;

            if (enumConstantDecl.DeclContext is EnumDecl enumDecl)
            {
                if (GetRemappedCursorName(enumDecl).StartsWith("__AnonymousEnum_"))
                {
                    isAnonymousEnum = true;
                    accessSpecifier = GetAccessSpecifierName(enumDecl);
                }
                typeName = GetRemappedTypeName(enumDecl, context: null, enumDecl.IntegerType, out var nativeTypeName);
            }
            else
            {
                typeName = GetRemappedTypeName(enumConstantDecl, context: null, enumConstantDecl.Type, out var nativeTypeName);
            }

            _outputBuilder.WriteIndentation();

            if (isAnonymousEnum)
            {
                _outputBuilder.Write(accessSpecifier);
                _outputBuilder.Write(" const ");
                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');
            }

            _outputBuilder.Write(escapedName);

            if (enumConstantDecl.InitExpr != null)
            {
                _outputBuilder.Write(" = ");
                UncheckStmt(typeName, enumConstantDecl.InitExpr);
            }
            else if (isAnonymousEnum)
            {
                _outputBuilder.Write(" = ");

                if (IsUnsigned(typeName))
                {
                    _outputBuilder.Write(enumConstantDecl.UnsignedInitVal);
                }
                else
                {
                    _outputBuilder.Write(enumConstantDecl.InitVal);
                }
            }

            _outputBuilder.WriteLine(isAnonymousEnum ? ';' : ',');
        }

        private void VisitEnumDecl(EnumDecl enumDecl)
        {
            var accessSpecifier = GetAccessSpecifierName(enumDecl);
            var name = GetRemappedCursorName(enumDecl);
            var escapedName = EscapeName(name);
            var isAnonymousEnum = false;

            if (name.StartsWith("__AnonymousEnum_"))
            {
                isAnonymousEnum = true;
                name = _config.MethodClassName;
            }

            StartUsingOutputBuilder(name);
            {
                if (!isAnonymousEnum)
                {
                    var typeName = GetRemappedTypeName(enumDecl, context: null, enumDecl.IntegerType, out var nativeTypeName);
                    AddNativeTypeNameAttribute(nativeTypeName);

                    _outputBuilder.WriteIndented(accessSpecifier);
                    _outputBuilder.Write(" enum ");
                    _outputBuilder.Write(escapedName);

                    if (!typeName.Equals("int"))
                    {
                        _outputBuilder.Write(" : ");
                        _outputBuilder.Write(typeName);
                    }

                    _outputBuilder.NeedsNewline = true;
                    _outputBuilder.WriteBlockStart();
                }

                Visit(enumDecl.Enumerators);
                Visit(enumDecl.Decls, excludedCursors: enumDecl.Enumerators);

                if (!isAnonymousEnum)
                {
                    _outputBuilder.WriteBlockEnd();
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

            var accessSpecifier = GetAccessSpecifierName(fieldDecl);
            var name = GetRemappedCursorName(fieldDecl);
            var escapedName = EscapeName(name);

            var type = fieldDecl.Type;
            var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out var nativeTypeName);

            if (fieldDecl.Parent.IsUnion)
            {
                _outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
            }
            AddNativeTypeNameAttribute(nativeTypeName);

            _outputBuilder.WriteIndented(accessSpecifier);
            _outputBuilder.Write(' ');

            if (NeedsNewKeyword(name))
            {
                _outputBuilder.Write("new ");
            }

            if (type.CanonicalType is ConstantArrayType constantArrayType)
            {
                if (IsSupportedFixedSizedBufferType(typeName))
                {
                    _outputBuilder.Write("fixed ");
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                    _outputBuilder.Write('[');
                    _outputBuilder.Write(Math.Max(constantArrayType.Size, 1));

                    var elementType = constantArrayType.ElementType;

                    while (elementType.CanonicalType is ConstantArrayType subConstantArrayType)
                    {
                        _outputBuilder.Write(" * ");
                        _outputBuilder.Write(Math.Max(subConstantArrayType.Size, 1));

                        elementType = subConstantArrayType.ElementType;
                    }

                    _outputBuilder.Write(']');
                }
                else
                {
                    _outputBuilder.Write(GetArtificialFixedSizedBufferName(fieldDecl));
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                }
            }
            else
            {
                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(escapedName);
            }

            _outputBuilder.WriteSemicolon();
            _outputBuilder.WriteNewline();
        }

        private void VisitFunctionDecl(FunctionDecl functionDecl)
        {
            if (IsExcluded(functionDecl))
            {
                return;
            }

            var accessSppecifier = GetAccessSpecifierName(functionDecl);
            var name = GetRemappedCursorName(functionDecl);

            if (!(functionDecl.DeclContext is CXXRecordDecl cxxRecordDecl))
            {
                cxxRecordDecl = null;
                StartUsingOutputBuilder(_config.MethodClassName);
            }

            WithAttributes("*");
            WithAttributes(name);

            WithUsings("*");
            WithUsings(name);

            var type = functionDecl.Type;
            var callConv = CXCallingConv.CXCallingConv_Invalid;

            if (type is AttributedType attributedType)
            {
                type = attributedType.ModifiedType;
                callConv = attributedType.Handle.FunctionTypeCallingConv;
            }

            if (type is FunctionType functionType)
            {
                if (callConv == CXCallingConv.CXCallingConv_Invalid)
                {
                    callConv = functionType.CallConv;
                }
            }

            var cxxMethodDecl = functionDecl as CXXMethodDecl;
            var body = functionDecl.Body;

            var isVirtual = (cxxMethodDecl != null) && cxxMethodDecl.IsVirtual;
            var escapedName = isVirtual ? PrefixAndStripName(name) : EscapeAndStripName(name);

            if (isVirtual)
            {
                Debug.Assert(!_config.GeneratePreviewCodeFnptr);

                _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                var callingConventionName = GetCallingConventionName(functionDecl, callConv, name, isForFnptr: false);

                _outputBuilder.WriteIndented("[UnmanagedFunctionPointer");

                if (callingConventionName != "Winapi")
                {
                    _outputBuilder.Write("(CallingConvention.");
                    _outputBuilder.Write(callingConventionName);
                    _outputBuilder.Write(')');
                }

                _outputBuilder.WriteLine(']');
            }
            else if (body is null)
            {
                _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                _outputBuilder.WriteIndented("[DllImport(");

                WithLibraryPath(name);

                _outputBuilder.Write(", ");

                var callingConventionName = GetCallingConventionName(functionDecl, callConv, name, isForFnptr: false);

                if (callingConventionName != "Winapi")
                {
                    _outputBuilder.Write("CallingConvention = CallingConvention.");
                    _outputBuilder.Write(callingConventionName);
                    _outputBuilder.Write(", ");
                }

                var entryPoint = (cxxMethodDecl is null) ? GetCursorName(functionDecl) : cxxMethodDecl.Handle.Mangling.CString;

                if (entryPoint != escapedName)
                {
                    _outputBuilder.Write("EntryPoint = \"");
                    _outputBuilder.Write(entryPoint);
                    _outputBuilder.Write("\", ");
                }

                _outputBuilder.Write("ExactSpelling = true");
                WithSetLastError(name);
                _outputBuilder.WriteLine(")]");
            }

            var returnType = functionDecl.ReturnType;
            var returnTypeName = GetRemappedTypeName(functionDecl, cxxRecordDecl, returnType, out var nativeTypeName);

            if ((isVirtual || (body is null)) && (returnTypeName == "bool"))
            {
                // bool is not blittable, so we shouldn't use it for P/Invoke signatures
                returnTypeName = "byte";
                nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? "bool" : nativeTypeName;
            }

            AddNativeTypeNameAttribute(nativeTypeName, attributePrefix: "return: ");

            _outputBuilder.WriteIndented(accessSppecifier);

            if (isVirtual)
            {
                _outputBuilder.Write(" delegate");
            }
            else if ((body is null) || (cxxMethodDecl is null) || cxxMethodDecl.IsStatic)
            {
                _outputBuilder.Write(" static");

                if (body is null)
                {
                    _outputBuilder.Write(" extern");
                }
            }

            _outputBuilder.Write(' ');

            if (!isVirtual)
            {
                if (NeedsNewKeyword(escapedName, functionDecl.Parameters))
                {
                    _outputBuilder.Write("new ");
                }

                if (IsUnsafe(functionDecl))
                {
                    if (cxxRecordDecl is null)
                    {
                        _isMethodClassUnsafe = true;
                    }
                    else if (!IsUnsafe(cxxRecordDecl))
                    {
                        _outputBuilder.Write("unsafe ");
                    }
                }
            }

            var needsReturnFixup = isVirtual && NeedsReturnFixup(cxxMethodDecl);

            if (!(functionDecl is CXXConstructorDecl))
            {
                _outputBuilder.Write(returnTypeName);

                if (needsReturnFixup)
                {
                    _outputBuilder.Write('*');
                }

                _outputBuilder.Write(' ');
            }

            _outputBuilder.Write(escapedName);
            _outputBuilder.Write('(');

            if (isVirtual)
            {
                Debug.Assert(cxxRecordDecl != null);

                if (!IsPrevContextDecl<CXXRecordDecl>(out CXXRecordDecl thisCursor))
                {
                    thisCursor = cxxRecordDecl;
                }

                var cxxRecordDeclName = GetRemappedCursorName(thisCursor);
                _outputBuilder.Write(EscapeName(cxxRecordDeclName));
                _outputBuilder.Write("* pThis");

                if (needsReturnFixup)
                {
                    _outputBuilder.Write(", ");
                    _outputBuilder.Write(returnTypeName);
                    _outputBuilder.Write("* _result");
                }

                if (functionDecl.Parameters.Any())
                {
                    _outputBuilder.Write(", ");
                }
            }

            Visit(functionDecl.Parameters);

            _outputBuilder.Write(')');

            if ((body is null) || isVirtual)
            {
                _outputBuilder.WriteSemicolon();
                _outputBuilder.WriteNewline();
            }
            else
            {
                _outputBuilder.NeedsNewline = true;

                int firstCtorInitializer = functionDecl.Parameters.Any() ? (functionDecl.CursorChildren.IndexOf(functionDecl.Parameters.Last()) + 1) : 0;
                int lastCtorInitializer = (functionDecl.Body != null) ? functionDecl.CursorChildren.IndexOf(functionDecl.Body) : functionDecl.CursorChildren.Count;

                _outputBuilder.WriteBlockStart();

                if (functionDecl is CXXConstructorDecl cxxConstructorDecl)
                {
                    VisitCtorInitializers(cxxConstructorDecl, firstCtorInitializer, lastCtorInitializer);
                }

                if (body is CompoundStmt compoundStmt)
                {
                    VisitStmts(compoundStmt.Body);
                }
                else
                {
                    _outputBuilder.WriteIndentation();
                    Visit(body);
                }

                _outputBuilder.WriteSemicolonIfNeeded();
                _outputBuilder.WriteNewlineIfNeeded();
                _outputBuilder.WriteBlockEnd();
            }
            _outputBuilder.NeedsNewline = true;

            Visit(functionDecl.Decls, excludedCursors: functionDecl.Parameters);

            if (cxxRecordDecl is null)
            {
                StopUsingOutputBuilder();
            }

            void VisitCtorInitializers(CXXConstructorDecl cxxConstructorDecl, int firstCtorInitializer, int lastCtorInitializer)
            {
                for (int i = firstCtorInitializer; i < lastCtorInitializer; i++)
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

                    if ((memberInit is CastExpr castExpr) && (castExpr.SubExprAsWritten is DeclRefExpr declRefExpr))
                    {
                        memberInitName = GetRemappedCursorName(declRefExpr.Decl);
                    }
                    _outputBuilder.WriteIndentation();

                    if (memberRefName.Equals(memberInitName))
                    {
                        _outputBuilder.Write("this");
                        _outputBuilder.Write('.');
                    }

                    Visit(memberRef);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write('=');
                    _outputBuilder.Write(' ');

                    var memberRefTypeName = GetRemappedTypeName(memberRef, context: null, memberRef.Type, out var memberRefNativeTypeName);

                    UncheckStmt(memberRefTypeName, memberInit);

                    _outputBuilder.WriteSemicolon();
                    _outputBuilder.WriteNewline();
                }
            }
        }

        private void VisitFunctionTemplateDecl(FunctionTemplateDecl functionTemplateDecl)
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Function templates are not supported: '{GetCursorQualifiedName(functionTemplateDecl)}'. Generated bindings may be incomplete.", functionTemplateDecl);
        }

        private void VisitLinkageSpecDecl(LinkageSpecDecl linkageSpecDecl)
        {
            foreach (var cursor in linkageSpecDecl.CursorChildren)
            {
                Visit(cursor);
            }
        }

        private void VisitNamespaceDecl(NamespaceDecl namespaceDecl)
        {
            // We don't currently include the namespace name anywhere in the
            // generated bindings. We might want to in the future...

            foreach (var cursor in namespaceDecl.CursorChildren)
            {
                Visit(cursor);
            }
        }

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl)
        {
            if (IsExcluded(parmVarDecl))
            {
                return;
            }

            if (IsPrevContextDecl<FunctionDecl>(out var functionDecl))
            {
                ForFunctionDecl(parmVarDecl, functionDecl);
            }
            else if (IsPrevContextDecl<TypedefDecl>(out var typedefDecl))
            {
                ForTypedefDecl(parmVarDecl, typedefDecl);
            }
            else
            {
                IsPrevContextDecl<Decl>(out var previousContext);
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported parameter variable declaration parent: '{previousContext.CursorKindSpelling}'. Generated bindings may be incomplete.", previousContext);
            }

            void ForFunctionDecl(ParmVarDecl parmVarDecl, FunctionDecl functionDecl)
            {
                var type = parmVarDecl.Type;
                var typeName = GetRemappedTypeName(parmVarDecl, context: null, type, out var nativeTypeName);

                if ((((functionDecl is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsVirtual) || (functionDecl.Body is null)) && (typeName == "bool"))
                {
                    // bool is not blittable, so we shouldn't use it for P/Invoke signatures
                    typeName = "byte";
                    nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? "bool" : nativeTypeName;
                }

                AddNativeTypeNameAttribute(nativeTypeName, prefix: "", postfix: " ");
                AddCppAttributes(parmVarDecl, prefix: "", postfix: " ");

                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');

                var name = GetRemappedCursorName(parmVarDecl);
                var escapedName = EscapeName(name);

                _outputBuilder.Write(escapedName);

                var parameters = functionDecl.Parameters;
                var index = parameters.IndexOf(parmVarDecl);
                var lastIndex = parameters.Count - 1;

                if (name.Equals("param"))
                {
                    _outputBuilder.Write(index);
                }

                if (parmVarDecl.HasDefaultArg)
                {
                    _outputBuilder.Write(" = ");
                    UncheckStmt(typeName, parmVarDecl.DefaultArg);
                }

                if (index != lastIndex)
                {
                    _outputBuilder.Write(", ");
                }
            }

            void ForTypedefDecl(ParmVarDecl parmVarDecl, TypedefDecl typedefDecl)
            {
                var type = parmVarDecl.Type;
                var typeName = GetRemappedTypeName(parmVarDecl, context: null, type, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName, prefix: "", postfix: " ");
                AddCppAttributes(parmVarDecl, prefix: "", postfix: " ");

                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');

                var name = GetRemappedCursorName(parmVarDecl);
                _outputBuilder.Write(EscapeName(name));

                var parameters = typedefDecl.CursorChildren.OfType<ParmVarDecl>().ToList();
                var index = parameters.IndexOf(parmVarDecl);
                var lastIndex = parameters.Count - 1;

                if (name.Equals("param"))
                {
                    _outputBuilder.Write(index);
                }

                if (parmVarDecl.HasDefaultArg)
                {
                    _outputBuilder.Write(" = ");
                    UncheckStmt(typeName, parmVarDecl.DefaultArg);
                }

                if (index != lastIndex)
                {
                    _outputBuilder.Write(", ");
                }
            }
        }

        private void VisitRecordDecl(RecordDecl recordDecl)
        {
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

                var alignment = recordDecl.TypeForDecl.Handle.AlignOf;
                var maxAlignm = recordDecl.Fields.Any() ? recordDecl.Fields.Max((fieldDecl) => fieldDecl.Type.Handle.AlignOf) : alignment;

                if ((_testOutputBuilder != null) && !recordDecl.IsAnonymousStructOrUnion && !(recordDecl.DeclContext is RecordDecl))
                {
                    _testOutputBuilder.WriteIndented("/// <summary>Provides validation of the <see cref=\"");
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.WriteLine("\" /> struct.</summary>");
                    _testOutputBuilder.WriteIndented("public static unsafe class ");
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.WriteLine("Tests");
                    _testOutputBuilder.WriteBlockStart();
                }

                if (recordDecl.IsUnion)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    _outputBuilder.WriteIndented("[StructLayout(LayoutKind.Explicit");

                    if (alignment < maxAlignm)
                    {
                        _outputBuilder.Write(", Pack = ");
                        _outputBuilder.Write(alignment);
                    }
                    _outputBuilder.WriteLine(")]");
                }
                else if (alignment < maxAlignm)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    _outputBuilder.WriteIndented("[StructLayout(LayoutKind.Sequential");

                    _outputBuilder.Write(", Pack = ");
                    _outputBuilder.Write(alignment);
                    _outputBuilder.WriteLine(")]");
                }

                if (TryGetUuid(recordDecl, out Guid uuid))
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                    _outputBuilder.WriteIndented("[Guid(\"");
                    _outputBuilder.Write(uuid.ToString("D", CultureInfo.InvariantCulture).ToUpperInvariant());
                    _outputBuilder.WriteLine("\")]");

                    var iidName = GetRemappedName($"IID_{nativeName}", recordDecl, tryRemapOperatorName: false);

                    _uuidsToGenerate.Add(iidName, uuid);

                    if (_testOutputBuilder != null)
                    {
                        _testOutputBuilder.AddUsingDirective("System");
                        _testOutputBuilder.AddUsingDirective($"static {_config.Namespace}.{_config.MethodClassName}");

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

                if ((cxxRecordDecl != null) && cxxRecordDecl.Bases.Any())
                {
                    var nativeTypeNameBuilder = new StringBuilder();

                    nativeTypeNameBuilder.Append(recordDecl.IsUnion ? "union " : "struct ");
                    nativeTypeNameBuilder.Append(nativeName);
                    nativeTypeNameBuilder.Append(" : ");

                    var baseName = GetCursorName(cxxRecordDecl.Bases[0].Referenced);
                    nativeTypeNameBuilder.Append(baseName);

                    for (int i = 1; i < cxxRecordDecl.Bases.Count; i++)
                    {
                        nativeTypeNameBuilder.Append(", ");
                        baseName = GetCursorName(cxxRecordDecl.Bases[i].Referenced);
                        nativeTypeNameBuilder.Append(baseName);
                    }

                    AddNativeTypeNameAttribute(nativeTypeNameBuilder.ToString());
                    AddNativeInheritanceAttribute(GetCursorName(cxxRecordDecl.Bases.Last().Referenced));
                }

                _outputBuilder.WriteIndented(GetAccessSpecifierName(recordDecl));
                _outputBuilder.Write(' ');

                if (IsUnsafe(recordDecl))
                {
                    _outputBuilder.Write("unsafe ");
                }

                _outputBuilder.Write("partial struct ");
                _outputBuilder.Write(escapedName);
                _outputBuilder.WriteNewline();
                _outputBuilder.WriteBlockStart();

                if (hasVtbl)
                {
                    if (_config.GenerateExplicitVtbls)
                    {
                        _outputBuilder.WriteIndented("public Vtbl* lpVtbl");
                    }
                    else
                    {
                        _outputBuilder.WriteIndented("public void** lpVtbl");
                    }

                    _outputBuilder.WriteSemicolon();
                    _outputBuilder.WriteNewline();
                    _outputBuilder.NeedsNewline = true;
                }

                if (cxxRecordDecl != null)
                {
                    foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                    {
                        var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);

                        if (HasFields(baseCxxRecordDecl))
                        {
                            _outputBuilder.WriteIndented(GetAccessSpecifierName(baseCxxRecordDecl));
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write(GetRemappedCursorName(baseCxxRecordDecl));
                            _outputBuilder.Write(' ');

                            var baseFieldName = GetAnonymousName(cxxBaseSpecifier, "Base");
                            baseFieldName = GetRemappedName(baseFieldName, cxxBaseSpecifier, tryRemapOperatorName: true);

                            _outputBuilder.Write(baseFieldName);
                            _outputBuilder.WriteSemicolon();
                            _outputBuilder.WriteNewline();

                            _outputBuilder.NeedsNewline = true;
                        }
                    }
                }

                if ((_testOutputBuilder != null) && !recordDecl.IsAnonymousStructOrUnion && !(recordDecl.DeclContext is RecordDecl))
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

                    long alignment32 = -1;
                    long alignment64 = -1;

                    GetTypeSize(recordDecl, recordDecl.TypeForDecl, ref alignment32, ref alignment64, out var size32, out var size64);

                    if (((size32 == 0) || (size64 == 0)) && !TryGetUuid(recordDecl, out _))
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"{escapedName} has a size of 0");
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

                foreach (var declaration in recordDecl.Decls)
                {
                    if (declaration is FieldDecl fieldDecl)
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

                        _outputBuilder.NeedsNewline = true;
                    }
                    else if ((declaration is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion)
                    {
                        VisitAnonymousRecordDecl(recordDecl, nestedRecordDecl);
                    }
                }

                if (cxxRecordDecl != null)
                {
                    foreach (var cxxConstructorDecl in cxxRecordDecl.Ctors)
                    {
                        Visit(cxxConstructorDecl);
                        _outputBuilder.NeedsNewline = true;
                    }

                    if (cxxRecordDecl.HasUserDeclaredDestructor)
                    {
                        Visit(cxxRecordDecl.Destructor);
                        _outputBuilder.NeedsNewline = true;
                    }

                    if (hasVtbl)
                    {
                        OutputDelegateSignatures(cxxRecordDecl, cxxRecordDecl, hitsPerName: new Dictionary<string, int>());
                    }
                }

                var excludedCursors = recordDecl.Fields.AsEnumerable<Cursor>();

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
                        _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                    }

                    if (!_config.GeneratePreviewCodeFnptr)
                    {
                        _outputBuilder.AddUsingDirective("System");
                        _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    }

                    int index = 0;
                    OutputVtblHelperMethods(cxxRecordDecl, cxxRecordDecl, ref index, hitsPerName: new Dictionary<string, int>());

                    if (_config.GenerateExplicitVtbls)
                    {
                        _outputBuilder.NeedsNewline = true;
                        _outputBuilder.WriteIndentedLine("public partial struct Vtbl");
                        _outputBuilder.WriteBlockStart();
                        OutputVtblEntries(cxxRecordDecl, cxxRecordDecl, hitsPerName: new Dictionary<string, int>());
                        _outputBuilder.WriteBlockEnd();
                    }
                }

                _outputBuilder.WriteBlockEnd();

                if ((_testOutputBuilder != null) && !recordDecl.IsAnonymousStructOrUnion && !(recordDecl.DeclContext is RecordDecl))
                {
                    _testOutputBuilder.WriteBlockEnd();
                }
            }
            StopUsingOutputBuilder();

            string FixupNameForMultipleHits(CXXMethodDecl cxxMethodDecl, Dictionary<string, int> hitsPerName)
            {
                var remappedName = GetRemappedCursorName(cxxMethodDecl);

                if (hitsPerName.TryGetValue(remappedName, out int hits))
                {
                    hitsPerName[remappedName] = (hits + 1);

                    var name = GetCursorName(cxxMethodDecl);
                    var remappedNames = (Dictionary<string, string>)_config.RemappedNames;

                    remappedNames[name] = $"{remappedName}{hits}";
                }
                else
                {
                    hitsPerName.Add(remappedName, 1);
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
                    if ((decl is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion && HasFields(nestedRecordDecl))
                    {
                        return true;
                    }
                }

                if (recordDecl is CXXRecordDecl cxxRecordDecl)
                {
                    foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                    {
                        var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);

                        if (HasFields(baseCxxRecordDecl))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            void OutputDelegateSignatures(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, Dictionary<string, int> hitsPerName)
            {
                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);
                    OutputDelegateSignatures(rootCxxRecordDecl, baseCxxRecordDecl, hitsPerName);
                }

                foreach (var cxxMethodDecl in cxxRecordDecl.Methods)
                {
                    if (!cxxMethodDecl.IsVirtual || IsExcluded(cxxMethodDecl))
                    {
                        continue;
                    }

                    if (!_config.GeneratePreviewCodeFnptr)
                    {
                        _outputBuilder.NeedsNewline = true;

                        var remappedName = FixupNameForMultipleHits(cxxMethodDecl, hitsPerName);
                        Debug.Assert(CurrentContext == rootCxxRecordDecl);
                        Visit(cxxMethodDecl);
                        RestoreNameForMultipleHits(cxxMethodDecl, hitsPerName, remappedName);
                    }
                }
            }

            void OutputMethods(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
            {
                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);
                    OutputMethods(rootCxxRecordDecl, baseCxxRecordDecl);
                }

                foreach (var cxxMethodDecl in cxxRecordDecl.Methods)
                {
                    if (cxxMethodDecl.IsVirtual || (cxxMethodDecl is CXXConstructorDecl) || (cxxMethodDecl is CXXDestructorDecl))
                    {
                        continue;
                    }

                    Debug.Assert(CurrentContext == rootCxxRecordDecl);
                    Visit(cxxMethodDecl);
                    _outputBuilder.NeedsNewline = true;
                }
            }

            void OutputVtblEntries(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, Dictionary<string, int> hitsPerName)
            {
                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);
                    OutputVtblEntries(rootCxxRecordDecl, baseCxxRecordDecl, hitsPerName);
                }

                var cxxMethodDecls = cxxRecordDecl.Methods;

                if (cxxMethodDecls.Count != 0)
                {
                    foreach (var cxxMethodDecl in cxxMethodDecls)
                    {
                        OutputVtblEntry(rootCxxRecordDecl, cxxMethodDecl, hitsPerName);
                        _outputBuilder.NeedsNewline = true;
                    }
                }
            }

            void OutputVtblEntry(CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl, Dictionary<string, int> hitsPerName)
            {
                if (!cxxMethodDecl.IsVirtual || IsExcluded(cxxMethodDecl))
                {
                    return;
                }

                var cxxMethodDeclTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, cxxMethodDecl.Type, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName);

                var accessSpecifier = GetAccessSpecifierName(cxxMethodDecl);
                var remappedName = FixupNameForMultipleHits(cxxMethodDecl, hitsPerName);
                var name = GetRemappedCursorName(cxxMethodDecl);
                RestoreNameForMultipleHits(cxxMethodDecl, hitsPerName, remappedName);

                _outputBuilder.WriteIndented(accessSpecifier);
                _outputBuilder.Write(' ');

                if (NeedsNewKeyword(remappedName))
                {
                    _outputBuilder.Write("new ");
                }

                _outputBuilder.Write(cxxMethodDeclTypeName);
                _outputBuilder.Write(' ');

                _outputBuilder.Write(EscapeAndStripName(name));

                _outputBuilder.WriteSemicolon();
                _outputBuilder.WriteNewline();
            }

            void OutputVtblHelperMethod(CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl, ref int vtblIndex, Dictionary<string, int> hitsPerName)
            {
                if (!cxxMethodDecl.IsVirtual)
                {
                    return;
                }

                if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
                {
                    if (!isExcludedByConflictingDefinition)
                    {
                        vtblIndex += 1;
                    }
                    return;
                }

                var currentContext = _context.AddLast(cxxMethodDecl);

                if (_config.GenerateAggressiveInlining)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                    _outputBuilder.WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                }

                var accessSpecifier = GetAccessSpecifierName(cxxMethodDecl);
                var returnType = cxxMethodDecl.ReturnType;
                var returnTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, returnType, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName, attributePrefix: "return: ");

                _outputBuilder.WriteIndented(accessSpecifier);
                _outputBuilder.Write(' ');

                var remappedName = FixupNameForMultipleHits(cxxMethodDecl, hitsPerName);
                var name = GetRemappedCursorName(cxxMethodDecl);
                RestoreNameForMultipleHits(cxxMethodDecl, hitsPerName, remappedName);

                if (NeedsNewKeyword(remappedName, cxxMethodDecl.Parameters))
                {
                    _outputBuilder.Write("new");
                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write(returnTypeName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeAndStripName(remappedName));
                _outputBuilder.Write('(');

                Visit(cxxMethodDecl.Parameters);

                _outputBuilder.WriteLine(')');
                _outputBuilder.WriteBlockStart();

                var needsReturnFixup = false;
                var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
                var escapedCXXRecordDeclName = EscapeName(cxxRecordDeclName);

                _outputBuilder.WriteIndentation();

                if (_config.GenerateCompatibleCode)
                {
                    _outputBuilder.Write("fixed (");
                    _outputBuilder.Write(escapedCXXRecordDeclName);
                    _outputBuilder.WriteLine("* pThis = &this)");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndentation();
                }

                if (returnType.Kind != CXTypeKind.CXType_Void)
                {
                    needsReturnFixup = NeedsReturnFixup(cxxMethodDecl);

                    if (needsReturnFixup)
                    {
                        _outputBuilder.Write(returnTypeName);
                        _outputBuilder.Write(" result");
                        _outputBuilder.WriteSemicolon();
                        _outputBuilder.WriteNewline();
                        _outputBuilder.WriteIndentation();
                    }

                    _outputBuilder.Write("return ");
                }

                if (needsReturnFixup)
                {
                    _outputBuilder.Write('*');
                }

                if (!_config.GeneratePreviewCodeFnptr)
                {
                    _outputBuilder.Write("Marshal.GetDelegateForFunctionPointer<");
                    _outputBuilder.Write(PrefixAndStripName(name));
                    _outputBuilder.Write(">(");
                }

                if (_config.GenerateExplicitVtbls)
                {
                    _outputBuilder.Write("lpVtbl->");
                    _outputBuilder.Write(EscapeAndStripName(name));
                }
                else
                {
                    var cxxMethodDeclTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, cxxMethodDecl.Type, out var _);

                    if (_config.GeneratePreviewCodeFnptr)
                    {
                        _outputBuilder.Write('(');
                    }

                    _outputBuilder.Write('(');
                    _outputBuilder.Write(cxxMethodDeclTypeName);
                    _outputBuilder.Write(")(lpVtbl[");
                    _outputBuilder.Write(vtblIndex);
                    _outputBuilder.Write("])");

                    if (_config.GeneratePreviewCodeFnptr)
                    {
                        _outputBuilder.Write(')');
                    }
                }

                if (!_config.GeneratePreviewCodeFnptr)
                {
                    _outputBuilder.Write(')');
                }

                _outputBuilder.Write('(');

                if (_config.GenerateCompatibleCode)
                {
                    _outputBuilder.Write("pThis");
                }
                else
                {
                    _outputBuilder.Write('(');
                    _outputBuilder.Write(escapedCXXRecordDeclName);
                    _outputBuilder.Write("*)Unsafe.AsPointer(ref this)");
                }

                if (needsReturnFixup)
                {
                    _outputBuilder.Write(", &result");
                }

                var parmVarDecls = cxxMethodDecl.Parameters;

                for (int index = 0; index < parmVarDecls.Count; index++)
                {
                    _outputBuilder.Write(", ");

                    var parmVarDeclName = GetRemappedCursorName(parmVarDecls[index]);
                    var escapedParmVarDeclName = EscapeName(parmVarDeclName);
                    _outputBuilder.Write(escapedParmVarDeclName);

                    if (parmVarDeclName.Equals("param"))
                    {
                        _outputBuilder.Write(index);
                    }
                }

                _outputBuilder.Write(')');

                if (returnTypeName == "bool")
                {
                    _outputBuilder.Write(" != 0");
                }

                _outputBuilder.WriteSemicolon();
                _outputBuilder.WriteNewline();

                if (_config.GenerateCompatibleCode)
                {
                    _outputBuilder.WriteBlockEnd();
                }

                _outputBuilder.WriteBlockEnd();
                vtblIndex += 1;

                Debug.Assert(_context.Last == currentContext);
                _context.RemoveLast();
            }

            void OutputVtblHelperMethods(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, ref int index, Dictionary<string, int> hitsPerName)
            {
                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);
                    OutputVtblHelperMethods(rootCxxRecordDecl, baseCxxRecordDecl, ref index, hitsPerName);
                }

                var cxxMethodDecls = cxxRecordDecl.Methods;
                var outputBuilder = _outputBuilder;

                foreach (var cxxMethodDecl in cxxMethodDecls)
                {
                    _outputBuilder.NeedsNewline = true;
                    OutputVtblHelperMethod(rootCxxRecordDecl, cxxMethodDecl, ref index, hitsPerName);
                }
            }

            void RestoreNameForMultipleHits(CXXMethodDecl cxxMethodDecl, Dictionary<string, int> hitsPerName, string remappedName)
            {
                if (hitsPerName[remappedName] != 1)
                {
                    var name = GetCursorName(cxxMethodDecl);
                    var remappedNames = (Dictionary<string, string>)_config.RemappedNames;

                    if (name.Equals(remappedName))
                    {
                        remappedNames.Remove(name);
                    }
                    else
                    {
                        remappedNames[name] = remappedName;
                    }
                }
            }

            void VisitAnonymousRecordDecl(RecordDecl recordDecl, RecordDecl nestedRecordDecl)
            {
                var nestedRecordDeclFieldName = GetRemappedCursorName(nestedRecordDecl);

                if (nestedRecordDeclFieldName.StartsWith("_"))
                {
                    int suffixLength = 0;

                    if (nestedRecordDeclFieldName.EndsWith("_e__Union"))
                    {
                        suffixLength = 10;
                    }
                    else if (nestedRecordDeclFieldName.EndsWith("_e__Struct"))
                    {
                        suffixLength = 11;
                    }

                    if (suffixLength != 0)
                    {
                        nestedRecordDeclFieldName = nestedRecordDeclFieldName.Substring(1, nestedRecordDeclFieldName.Length - suffixLength);
                    }
                }

                var nestedRecordDeclName = GetRemappedTypeName(nestedRecordDecl, context: null, nestedRecordDecl.TypeForDecl, out string nativeTypeName);

                if (recordDecl.IsUnion)
                {
                    _outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
                }
                AddNativeTypeNameAttribute(nativeTypeName);

                _outputBuilder.WriteIndented("public ");
                _outputBuilder.Write(nestedRecordDeclName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(nestedRecordDeclFieldName);
                _outputBuilder.WriteSemicolon();
                _outputBuilder.WriteNewline();
                _outputBuilder.NeedsNewline = true;

                if (!recordDecl.IsAnonymousStructOrUnion)
                {
                    VisitAnonymousRecordDeclFields(recordDecl, nestedRecordDecl, nestedRecordDeclName, nestedRecordDeclFieldName);
                }
            }

            void VisitAnonymousRecordDeclFields(RecordDecl rootRecordDecl, RecordDecl anonymousRecordDecl, string contextType, string contextName)
            {
                if (_config.ExcludeAnonymousFieldHelpers)
                {
                    return;
                }

                foreach (var declaration in anonymousRecordDecl.Decls)
                {
                    if (declaration is FieldDecl fieldDecl)
                    {
                        var type = fieldDecl.Type;

                        var accessSpecifier = GetAccessSpecifierName(anonymousRecordDecl);
                        var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out var fieldNativeTypeName);
                        var name = GetRemappedCursorName(fieldDecl);
                        var escapedName = EscapeName(name);

                        _outputBuilder.WriteIndented(accessSpecifier);
                        _outputBuilder.Write(' ');

                        var isFixedSizedBuffer = (type.CanonicalType is ConstantArrayType);
                        var generateCompatibleCode = _config.GenerateCompatibleCode;

                        if (!fieldDecl.IsBitField && (!isFixedSizedBuffer || generateCompatibleCode))
                        {
                            _outputBuilder.Write("ref ");
                        }

                        if (type.CanonicalType is RecordType recordType)
                        {
                            var recordDecl = recordType.Decl;

                            while ((recordDecl.DeclContext is RecordDecl parentRecordDecl) && (parentRecordDecl != rootRecordDecl))
                            {
                                var parentRecordDeclName = GetRemappedCursorName(parentRecordDecl);
                                var escapedParentRecordDeclName = EscapeName(parentRecordDeclName);

                                _outputBuilder.Write(escapedParentRecordDeclName);
                                _outputBuilder.Write('.');

                                recordDecl = parentRecordDecl;
                            }
                        }

                        var isSupportedFixedSizedBufferType = isFixedSizedBuffer && IsSupportedFixedSizedBufferType(typeName);

                        if (isFixedSizedBuffer)
                        {
                            if (!generateCompatibleCode)
                            {
                                _outputBuilder.AddUsingDirective("System");
                                _outputBuilder.Write("Span<");
                            }
                            else if(!isSupportedFixedSizedBufferType)
                            {
                                _outputBuilder.Write(contextType);
                                _outputBuilder.Write('.');
                                typeName = GetArtificialFixedSizedBufferName(fieldDecl);
                            }
                        }

                        _outputBuilder.Write(typeName);

                        if (isFixedSizedBuffer && !generateCompatibleCode)
                        {
                            _outputBuilder.Write('>');
                        }

                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(escapedName);

                        generateCompatibleCode |= ((type.CanonicalType is PointerType) || (type.CanonicalType is ReferenceType)) && ((typeName != "IntPtr") && (typeName != "UIntPtr"));

                        _outputBuilder.WriteNewline();
                        _outputBuilder.WriteBlockStart();

                        if (_config.GenerateAggressiveInlining)
                        {
                            _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                            _outputBuilder.WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        }

                        _outputBuilder.WriteIndentedLine("get");
                        _outputBuilder.WriteBlockStart();

                        if (fieldDecl.IsBitField)
                        {
                            _outputBuilder.WriteIndented("return ");
                            _outputBuilder.Write(contextName);
                            _outputBuilder.Write('.');
                            _outputBuilder.Write(escapedName);
                            _outputBuilder.WriteSemicolon();
                            _outputBuilder.WriteNewline();
                            _outputBuilder.WriteBlockEnd();

                            _outputBuilder.WriteNewline();

                            if (_config.GenerateAggressiveInlining)
                            {
                                _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                                _outputBuilder.WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                            }

                            _outputBuilder.WriteIndentedLine("set");
                            _outputBuilder.WriteBlockStart();
                            _outputBuilder.WriteIndented(contextName);
                            _outputBuilder.Write('.');
                            _outputBuilder.Write(escapedName);
                            _outputBuilder.Write(" = value");
                            _outputBuilder.WriteSemicolon();
                            _outputBuilder.WriteNewline();
                        }
                        else if (generateCompatibleCode)
                        {
                            _outputBuilder.WriteIndented("fixed (");
                            _outputBuilder.Write(contextType);
                            _outputBuilder.Write("* pField = &");
                            _outputBuilder.Write(contextName);
                            _outputBuilder.WriteLine(')');
                            _outputBuilder.WriteBlockStart();
                            _outputBuilder.WriteIndented("return ref pField->");
                            _outputBuilder.Write(escapedName);

                            if (isSupportedFixedSizedBufferType)
                            {
                                _outputBuilder.Write("[0]");
                            }

                            _outputBuilder.WriteSemicolon();
                            _outputBuilder.WriteNewline();
                            _outputBuilder.WriteBlockEnd();
                        }
                        else
                        {
                            _outputBuilder.WriteIndented("return ");

                            if (!isFixedSizedBuffer)
                            {
                                _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                                _outputBuilder.Write("ref MemoryMarshal.GetReference(");
                            }

                            if (!isFixedSizedBuffer || isSupportedFixedSizedBufferType)
                            {
                                _outputBuilder.Write("MemoryMarshal.CreateSpan(ref ");
                            }

                            _outputBuilder.Write(contextName);
                            _outputBuilder.Write('.');
                            _outputBuilder.Write(escapedName);

                            if (isFixedSizedBuffer)
                            {
                                if (isSupportedFixedSizedBufferType)
                                {
                                    _outputBuilder.Write("[0], ");
                                    _outputBuilder.Write(((ConstantArrayType)type.CanonicalType).Size);
                                }
                                else
                                {
                                    _outputBuilder.Write(".AsSpan(");
                                }
                            }
                            else
                            {
                                _outputBuilder.Write(", 1)");
                            }

                            _outputBuilder.Write(')');
                            _outputBuilder.WriteSemicolon();
                            _outputBuilder.WriteNewline();
                        }

                        _outputBuilder.WriteBlockEnd();
                        _outputBuilder.WriteBlockEnd();

                        _outputBuilder.NeedsNewline = true;
                    }
                    else if ((declaration is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion)
                    {
                        var nestedRecordDeclName = GetRemappedTypeName(nestedRecordDecl, context: null, nestedRecordDecl.TypeForDecl, out string nativeTypeName);
                        var name = GetRemappedCursorName(nestedRecordDecl);

                        if (name.StartsWith("_"))
                        {
                            int suffixLength = 0;

                            if (name.EndsWith("_e__Union"))
                            {
                                suffixLength = 10;
                            }
                            else if (name.EndsWith("_e__Struct"))
                            {
                                suffixLength = 11;
                            }

                            if (suffixLength != 0)
                            {
                                name = name.Substring(1, name.Length - suffixLength);
                            }
                        }
                        var escapedName = EscapeName(name);

                        VisitAnonymousRecordDeclFields(rootRecordDecl, nestedRecordDecl, $"{contextType}.{nestedRecordDeclName}", $"{contextName}.{escapedName}");
                    }
                }
            }

            void VisitBitfieldDecl(FieldDecl fieldDecl, Type[] types, RecordDecl recordDecl, string contextName, ref int index, ref long previousSize, ref long remainingBits)
            {
                Debug.Assert(fieldDecl.IsBitField);

                var outputBuilder = _outputBuilder;

                var type = fieldDecl.Type;
                var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out var nativeTypeName);

                if (string.IsNullOrWhiteSpace(nativeTypeName))
                {
                    nativeTypeName = typeName;
                }
                nativeTypeName += $" : {fieldDecl.BitWidthValue}";

                if (fieldDecl.Parent.IsUnion)
                {
                    _outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
                }
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
                        _outputBuilder.WriteIndented("public ");
                        _outputBuilder.Write(typeNameBacking);
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(bitfieldName);
                        _outputBuilder.WriteSemicolon();
                        _outputBuilder.WriteNewline();
                        _outputBuilder.NeedsNewline = true;
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

                AddNativeTypeNameAttribute(nativeTypeName);

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
                        bitwidthHexStringBacking += "L";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{canonicalTypeBacking.TypeClass}'. Generated bindings may be incomplete.", fieldDecl);
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
                        bitwidthHexString += "L";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{canonicalType.TypeClass}'. Generated bindings may be incomplete.", fieldDecl);
                        break;
                    }
                }

                canonicalType = type.CanonicalType;

                var accessSpecifier = GetAccessSpecifierName(fieldDecl);
                var name = GetRemappedCursorName(fieldDecl);
                var escapedName = EscapeName(name);

                _outputBuilder.WriteIndented(accessSpecifier);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');
                _outputBuilder.WriteLine(escapedName);
                _outputBuilder.WriteBlockStart();

                if (_config.GenerateAggressiveInlining)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                    _outputBuilder.WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                }

                _outputBuilder.WriteIndentedLine("get");
                _outputBuilder.WriteBlockStart();
                _outputBuilder.WriteIndented("return ");

                if ((currentSize < 4) || (canonicalTypeBacking != canonicalType))
                {
                    _outputBuilder.Write('(');
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(")(");
                }

                if (bitfieldOffset != 0)
                {
                    _outputBuilder.Write('(');
                }

                if (!string.IsNullOrWhiteSpace(contextName))
                {
                    _outputBuilder.Write(contextName);
                    _outputBuilder.Write('.');
                }
                _outputBuilder.Write(bitfieldName);

                if (bitfieldOffset != 0)
                {
                    _outputBuilder.Write(" >> ");
                    _outputBuilder.Write(bitfieldOffset);
                    _outputBuilder.Write(')');
                }

                _outputBuilder.Write(" & 0x");
                _outputBuilder.Write(bitwidthHexStringBacking);

                if ((currentSize < 4) || (canonicalTypeBacking != canonicalType))
                {
                    _outputBuilder.Write(')');
                }

                _outputBuilder.WriteSemicolon();
                _outputBuilder.WriteNewline();
                _outputBuilder.WriteBlockEnd();

                _outputBuilder.NeedsNewline = true;

                if (_config.GenerateAggressiveInlining)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                    _outputBuilder.WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                }

                _outputBuilder.WriteIndentedLine("set");
                _outputBuilder.WriteBlockStart();
                _outputBuilder.WriteIndentation();

                if (!string.IsNullOrWhiteSpace(contextName))
                {
                    _outputBuilder.Write(contextName);
                    _outputBuilder.Write('.');
                }
                _outputBuilder.Write(bitfieldName);

                _outputBuilder.Write(" = ");

                if (currentSize < 4)
                {
                    _outputBuilder.Write('(');
                    _outputBuilder.Write(typeNameBacking);
                    _outputBuilder.Write(")(");
                }

                _outputBuilder.Write('(');

                if (!string.IsNullOrWhiteSpace(contextName))
                {
                    _outputBuilder.Write(contextName);
                    _outputBuilder.Write('.');
                }
                _outputBuilder.Write(bitfieldName);

                _outputBuilder.Write(" & ~");

                if (bitfieldOffset != 0)
                {
                    _outputBuilder.Write('(');
                }

                _outputBuilder.Write("0x");
                _outputBuilder.Write(bitwidthHexStringBacking);

                if (bitfieldOffset != 0)
                {
                    _outputBuilder.Write(" << ");
                    _outputBuilder.Write(bitfieldOffset);
                    _outputBuilder.Write(')');
                }

                _outputBuilder.Write(") | ");

                if ((canonicalTypeBacking != canonicalType) && !(canonicalType is EnumType))
                {
                    _outputBuilder.Write('(');
                    _outputBuilder.Write(typeNameBacking);
                    _outputBuilder.Write(')');
                }

                _outputBuilder.Write('(');

                if (bitfieldOffset != 0)
                {
                    _outputBuilder.Write('(');
                }

                if (canonicalType is EnumType)
                {
                    _outputBuilder.Write('(');
                    _outputBuilder.Write(typeNameBacking);
                    _outputBuilder.Write(")(value)");
                }
                else
                {
                    _outputBuilder.Write("value");
                }

                _outputBuilder.Write(" & 0x");
                _outputBuilder.Write(bitwidthHexString);

                if (bitfieldOffset != 0)
                {
                    _outputBuilder.Write(") << ");
                    _outputBuilder.Write(bitfieldOffset);
                }

                _outputBuilder.Write(')');

                if (currentSize < 4)
                {
                    _outputBuilder.Write(')');
                }

                _outputBuilder.WriteSemicolon();
                _outputBuilder.WriteNewline();
                _outputBuilder.WriteBlockEnd();
                _outputBuilder.WriteBlockEnd();

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

                _outputBuilder.NeedsNewline = true;

                var alignment = recordDecl.TypeForDecl.Handle.AlignOf;
                var maxAlignm = recordDecl.Fields.Max((fieldDecl) => fieldDecl.Type.Handle.AlignOf);

                if (alignment < maxAlignm)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    _outputBuilder.WriteIndented("[StructLayout(LayoutKind.Sequential");
                    _outputBuilder.Write(", Pack = ");
                    _outputBuilder.Write(alignment);
                    _outputBuilder.WriteLine(")]");
                }

                var accessSpecifier = GetAccessSpecifierName(constantArray);
                var canonicalElementType = type.ElementType.CanonicalType;
                var isUnsafeElementType = ((canonicalElementType is PointerType) || (canonicalElementType is ReferenceType)) && ((typeName != "IntPtr") && (typeName != "UIntPtr"));

                _outputBuilder.WriteIndented(accessSpecifier);
                _outputBuilder.Write(' ');

                if (isUnsafeElementType)
                {
                    _outputBuilder.Write("unsafe ");
                }

                var name = GetArtificialFixedSizedBufferName(constantArray);
                var escapedName = EscapeName(name);

                _outputBuilder.Write("partial struct ");
                _outputBuilder.WriteLine(escapedName);
                _outputBuilder.WriteBlockStart();

                var totalSize = Math.Max(type.Size, 1);
                var sizePerDimension = new List<(long index, long size)>() {
                    (0, type.Size)
                };

                var elementType = type.ElementType;

                while (elementType.CanonicalType is ConstantArrayType subConstantArrayType)
                {
                    totalSize *= Math.Max(subConstantArrayType.Size, 1);
                    sizePerDimension.Add((0, Math.Max(subConstantArrayType.Size, 1)));
                    elementType = subConstantArrayType.ElementType;
                }

                for (long i = 0; i < totalSize; i++)
                {
                    _outputBuilder.WriteIndented("public ");
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(" e");

                    var dimension = sizePerDimension[0];
                    _outputBuilder.Write(dimension.index++);
                    sizePerDimension[0] = dimension;

                    for (int d = 1; d < sizePerDimension.Count; d++)
                    {
                        dimension = sizePerDimension[d];
                        _outputBuilder.Write('_');
                        _outputBuilder.Write(dimension.index);
                        sizePerDimension[d] = dimension;

                        var previousDimension = sizePerDimension[d - 1];

                        if (previousDimension.index == previousDimension.size)
                        {
                            previousDimension.index = 0;
                            dimension.index++;
                            sizePerDimension[d - 1] = previousDimension;
                            _outputBuilder.NeedsNewline = true;
                        }

                        sizePerDimension[d] = dimension;
                    }

                    if (_outputBuilder.NeedsNewline)
                    {
                        _outputBuilder.WriteSemicolon();
                        _outputBuilder.WriteNewline();
                        _outputBuilder.NeedsNewline = true;
                    }
                    else
                    {
                        _outputBuilder.WriteSemicolon();
                        _outputBuilder.WriteNewline();
                    }
                }

                _outputBuilder.NeedsNewline = true;
                _outputBuilder.WriteIndented("public ");

                var generateCompatibleCode = _config.GenerateCompatibleCode;

                if (generateCompatibleCode && !isUnsafeElementType)
                {
                    _outputBuilder.Write("unsafe ");
                }
                else if (!isUnsafeElementType)
                {
                    _outputBuilder.AddUsingDirective("System");
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                }

                _outputBuilder.Write("ref ");
                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');

                if (generateCompatibleCode || isUnsafeElementType)
                {
                    _outputBuilder.WriteLine("this[int index]");
                    _outputBuilder.WriteBlockStart();

                    if (_config.GenerateAggressiveInlining)
                    {
                        _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                        _outputBuilder.WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    }

                    _outputBuilder.WriteIndentedLine("get");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndented("fixed (");
                    _outputBuilder.Write(typeName);
                    _outputBuilder.WriteLine("* pThis = &e0)");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndented("return ref pThis[index]");
                    _outputBuilder.WriteSemicolon();
                    _outputBuilder.WriteNewline();
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();
                }
                else
                {
                    _outputBuilder.WriteLine("this[int index]");
                    _outputBuilder.WriteBlockStart();

                    if (_config.GenerateAggressiveInlining)
                    {
                        _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                        _outputBuilder.WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    }

                    _outputBuilder.WriteIndentedLine("get");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndented("return ref AsSpan(");

                    if (type.Size == 1)
                    {
                        _outputBuilder.Write("int.MaxValue");
                    }

                    _outputBuilder.Write(")[index]");
                    _outputBuilder.WriteSemicolon();
                    _outputBuilder.WriteNewline();
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();

                    _outputBuilder.NeedsNewline = true;

                    if (_config.GenerateAggressiveInlining)
                    {
                        _outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                        _outputBuilder.WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    }

                    _outputBuilder.WriteIndented("public Span<");
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write("> AsSpan(");

                    if (type.Size == 1)
                    {
                        _outputBuilder.Write("int length");
                    }

                    _outputBuilder.Write(") => MemoryMarshal.CreateSpan(ref e0, ");

                    if (type.Size == 1)
                    {
                        _outputBuilder.Write("length");
                    }
                    else
                    {
                        _outputBuilder.Write(totalSize);
                    }

                    _outputBuilder.Write(')');
                    _outputBuilder.WriteSemicolon();
                    _outputBuilder.WriteNewline();
                }

                _outputBuilder.WriteBlockEnd();
            }
        }

        private void VisitTranslationUnitDecl(TranslationUnitDecl translationUnitDecl)
        {
            foreach (var cursor in translationUnitDecl.CursorChildren)
            {
                Visit(cursor);
            }
        }

        private void VisitTypedefDecl(TypedefDecl typedefDecl)
        {
            ForUnderlyingType(typedefDecl, typedefDecl.UnderlyingType);

            void ForFunctionProtoType(TypedefDecl typedefDecl, FunctionProtoType functionProtoType, Type parentType)
            {
                if (_config.GeneratePreviewCodeFnptr)
                {
                    return;
                }

                var name = GetRemappedCursorName(typedefDecl);
                var escapedName = EscapeName(name);

                StartUsingOutputBuilder(name);
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                    var callingConventionName = GetCallingConventionName(typedefDecl, (parentType is AttributedType) ? parentType.Handle.FunctionTypeCallingConv : functionProtoType.CallConv, name, isForFnptr: false);

                    _outputBuilder.WriteIndented("[UnmanagedFunctionPointer");

                    if (callingConventionName != "Winapi")
                    {
                        _outputBuilder.Write("(CallingConvention.");
                        _outputBuilder.Write(callingConventionName);
                        _outputBuilder.Write(')');
                    }

                    _outputBuilder.WriteLine(']');

                    var returnType = functionProtoType.ReturnType;
                    var returnTypeName = GetRemappedTypeName(typedefDecl, context: null, returnType, out var nativeTypeName);
                    AddNativeTypeNameAttribute(nativeTypeName, attributePrefix: "return: ");

                    _outputBuilder.WriteIndented(GetAccessSpecifierName(typedefDecl));
                    _outputBuilder.Write(' ');

                    if (IsUnsafe(typedefDecl, functionProtoType))
                    {
                        _outputBuilder.Write("unsafe ");
                    }

                    _outputBuilder.Write("delegate ");
                    _outputBuilder.Write(returnTypeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                    _outputBuilder.Write('(');

                    Visit(typedefDecl.CursorChildren.OfType<ParmVarDecl>());

                    _outputBuilder.Write(')');
                    _outputBuilder.WriteSemicolon();
                    _outputBuilder.WriteNewline();
                }
                StopUsingOutputBuilder();
            }

            void ForPointeeType(TypedefDecl typedefDecl, Type parentType, Type pointeeType)
            {
                if (pointeeType is AttributedType attributedType)
                {
                    ForPointeeType(typedefDecl, attributedType, attributedType.ModifiedType);
                }
                else if (pointeeType is ElaboratedType elaboratedType)
                {
                    ForPointeeType(typedefDecl, elaboratedType, elaboratedType.NamedType);
                }
                else if (pointeeType is FunctionProtoType functionProtoType)
                {
                    ForFunctionProtoType(typedefDecl, functionProtoType, parentType);
                }
                else if (pointeeType is PointerType pointerType)
                {
                    ForPointeeType(typedefDecl, pointerType, pointerType.PointeeType);
                }
                else if (pointeeType is TypedefType typedefType)
                {
                    ForPointeeType(typedefDecl, typedefType, typedefType.Decl.UnderlyingType);
                }
                else if (!(pointeeType is ConstantArrayType) && !(pointeeType is BuiltinType) && !(pointeeType is TagType))
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported pointee type: '{pointeeType.TypeClass}'. Generating bindings may be incomplete.", typedefDecl);
                }
            }

            void ForUnderlyingType(TypedefDecl typedefDecl, Type underlyingType)
            {
                if (underlyingType is ArrayType arrayType)
                {
                    // Nothing to do for array types
                }
                else if (underlyingType is AttributedType attributedType)
                {
                    ForUnderlyingType(typedefDecl, attributedType.ModifiedType);
                }
                else if (underlyingType is BuiltinType builtinType)
                {
                    // Nothing to do for builtin types
                }
                else if (underlyingType is ElaboratedType elaboratedType)
                {
                    ForUnderlyingType(typedefDecl, elaboratedType.NamedType);
                }
                else if (underlyingType is FunctionProtoType functionProtoType)
                {
                    ForFunctionProtoType(typedefDecl, functionProtoType, parentType: null);
                }
                else if (underlyingType is PointerType pointerType)
                {
                    ForPointeeType(typedefDecl, parentType: null, pointerType.PointeeType);
                }
                else if (underlyingType is ReferenceType referenceType)
                {
                    ForPointeeType(typedefDecl, parentType: null, referenceType.PointeeType);
                }
                else if (underlyingType is TagType underlyingTagType)
                {
                    // See if there's a potential typedef remapping we want to log
                    if (_config.LogPotentialTypedefRemappings)
                    {
                        var typedefName = typedefDecl.UnderlyingDecl.Name;
                        var possibleNamesToRemap = new string[] { "_" + typedefName, "_tag" + typedefName, "tag" + typedefName };
                        var underlyingName = underlyingTagType.AsString;

                        foreach (var possibleNameToRemap in possibleNamesToRemap)
                        {
                            if (!_config.RemappedNames.ContainsKey(possibleNameToRemap))
                            {
                                if (possibleNameToRemap == underlyingName)
                                {
                                    AddDiagnostic(DiagnosticLevel.Info, $"Potential remap: {possibleNameToRemap}={typedefName}");
                                }
                            }
                        }
                    }
                }
                else if (underlyingType is TypedefType typedefType)
                {
                    ForUnderlyingType(typedefDecl, typedefType.Decl.UnderlyingType);
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported underlying type: '{underlyingType.TypeClass}'. Generating bindings may be incomplete.", typedefDecl);
                }
                return;
            }

            string GetUndecoratedName(Type type)
            {
                if (type is AttributedType attributedType)
                {
                    return GetUndecoratedName(attributedType.ModifiedType);
                }
                else if (type is ElaboratedType elaboratedType)
                {
                    return GetUndecoratedName(elaboratedType.NamedType);
                }
                else
                {
                    return type.AsString;
                }
            }
        }

        private void VisitVarDecl(VarDecl varDecl)
        {
            if (IsPrevContextStmt<DeclStmt>(out var declStmt))
            {
                ForDeclStmt(varDecl, declStmt);
            }
            else if (IsPrevContextDecl<TranslationUnitDecl>(out _) || IsPrevContextDecl<LinkageSpecDecl>(out _) || IsPrevContextDecl<RecordDecl>(out _))
            {
                if (!varDecl.HasInit)
                {
                    // Nothing to do if a top level const declaration doesn't have an initializer
                    return;
                }

                var type = varDecl.Type;
                var isMacroDefinitionRecord = false;

                var nativeName = GetCursorName(varDecl);
                if (nativeName.StartsWith("ClangSharpMacro_"))
                {
                    type = varDecl.Init.Type;
                    nativeName = nativeName.Substring("ClangSharpMacro_".Length);
                    isMacroDefinitionRecord = true;
                }

                var accessSpecifier = GetAccessSpecifierName(varDecl);
                var name = GetRemappedName(nativeName, varDecl, tryRemapOperatorName: false);
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

                if (_outputBuilder is null)
                {
                    StartUsingOutputBuilder(_config.MethodClassName);
                    openedOutputBuilder = true;

                    if (IsUnsafe(varDecl, type) && (!varDecl.HasInit || !IsStmtAsWritten<StringLiteral>(varDecl.Init, out _, removeParens: true)))
                    {
                        _isMethodClassUnsafe = true;
                    }
                }

                WithAttributes("*");
                WithAttributes(name);

                WithUsings("*");
                WithUsings(name);

                var typeName = GetRemappedTypeName(varDecl, context: null, type, out var nativeTypeName);

                if (typeName == "Guid")
                {
                    _generatedUuids.Add(name);
                }

                if (isMacroDefinitionRecord)
                {
                    var nativeTypeNameBuilder = new StringBuilder("#define");

                    nativeTypeNameBuilder.Append(' ');
                    nativeTypeNameBuilder.Append(nativeName);
                    nativeTypeNameBuilder.Append(' ');

                    var macroValue = GetSourceRangeContents(varDecl.TranslationUnit.Handle, varDecl.Init.Extent);
                    nativeTypeNameBuilder.Append(macroValue);

                    nativeTypeName = nativeTypeNameBuilder.ToString();
                }

                AddNativeTypeNameAttribute(nativeTypeName);

                _outputBuilder.WriteIndented(accessSpecifier);
                _outputBuilder.Write(' ');

                var isProperty = false;
                var isStringLiteral = false;

                if (IsStmtAsWritten<StringLiteral>(varDecl.Init, out var stringLiteral, removeParens: true))
                {
                    switch (stringLiteral.Kind)
                    {
                        case CX_CharacterKind.CX_CLK_Ascii:
                        case CX_CharacterKind.CX_CLK_UTF8:
                        {
                            _outputBuilder.AddUsingDirective("System");
                            _outputBuilder.Write("static ");

                            typeName = "ReadOnlySpan<byte>";
                            isProperty = true;
                            isStringLiteral = true;
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
                            _outputBuilder.Write("const ");

                            typeName = "string";
                            isStringLiteral = true;
                            break;
                        }

                        default:
                        {
                            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported string literal kind: '{stringLiteral.Kind}'. Generated bindings may be incomplete.", stringLiteral);
                            break;
                        }
                    }
                }
                else if ((type.IsLocalConstQualified || isMacroDefinitionRecord) && CanBeConstant(type, varDecl.Init))
                {
                    _outputBuilder.Write("const ");
                }
                else if ((varDecl.StorageClass == CX_StorageClass.CX_SC_Static) || openedOutputBuilder)
                {
                    _outputBuilder.Write("static ");

                    if (type.IsLocalConstQualified || isMacroDefinitionRecord)
                    {
                        _outputBuilder.Write("readonly ");
                    }
                }

                _outputBuilder.Write(typeName);

                if (!isStringLiteral && type is ArrayType)
                {
                    _outputBuilder.Write("[]");
                }

                _outputBuilder.Write(' ');

                _outputBuilder.Write(escapedName);

                if (varDecl.HasInit)
                {
                    _outputBuilder.Write(" =");

                    if (isProperty)
                    {
                        _outputBuilder.Write('>');
                    }

                    _outputBuilder.Write(' ');

                    if ((type.CanonicalType is PointerType pointerType) && (pointerType.PointeeType.CanonicalType is FunctionType) && isMacroDefinitionRecord)
                    {
                        _outputBuilder.Write('&');
                    }
                    UncheckStmt(typeName, varDecl.Init);
                }

                _outputBuilder.WriteSemicolon();
                _outputBuilder.WriteNewline();

                if (openedOutputBuilder)
                {
                    StopUsingOutputBuilder();
                }
                else
                {
                    _outputBuilder.NeedsNewline = true;
                }
            }
            else
            {
                IsPrevContextDecl<Decl>(out var previousContext);
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported variable declaration parent: '{previousContext.CursorKindSpelling}'. Generated bindings may be incomplete.", previousContext);
            }

            void ForDeclStmt(VarDecl varDecl, DeclStmt declStmt)
            {
                var name = GetRemappedCursorName(varDecl);
                var escapedName = EscapeName(name);

                if (varDecl == declStmt.Decls.First())
                {
                    var type = varDecl.Type;
                    var typeName = GetRemappedTypeName(varDecl, context: null, type, out var nativeTypeName);

                    _outputBuilder.Write(typeName);

                    if (type is ArrayType)
                    {
                        _outputBuilder.Write("[]");
                    }

                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write(escapedName);

                if (varDecl.HasInit)
                {
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write('=');
                    _outputBuilder.Write(' ');

                    var varDeclTypeName = GetRemappedTypeName(varDecl, context: null, varDecl.Type, out var varDeclNativeTypeName);
                    UncheckStmt(varDeclTypeName, varDecl.Init);
                }
            }

            bool CanBeConstant(Type type, Expr initExpr)
            {
                if (type is AttributedType attributedType)
                {
                    return CanBeConstant(attributedType.ModifiedType, initExpr);
                }
                else if (type is AutoType autoType)
                {
                    return CanBeConstant(autoType.CanonicalType, initExpr);
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
                            return IsConstant(initExpr);
                        }
                    }
                }
                else if (type is ElaboratedType elaboratedType)
                {
                    return CanBeConstant(elaboratedType.NamedType, initExpr);
                }
                else if (type is EnumType enumType)
                {
                    return CanBeConstant(enumType.Decl.IntegerType, initExpr);
                }
                else if (type is TypedefType typedefType)
                {
                    return CanBeConstant(typedefType.Decl.UnderlyingType, initExpr);
                }

                return false;
            }
        }

        bool IsConstant(Expr initExpr)
        {
            switch (initExpr.StmtClass)
            {
                // case CX_StmtClass.CX_StmtClass_BinaryConditionalOperator:

                case CX_StmtClass.CX_StmtClass_ConditionalOperator:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_AddrLabelExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr:
                // case CX_StmtClass.CX_StmtClass_ArraySubscriptExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayTypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_AsTypeExpr:
                // case CX_StmtClass.CX_StmtClass_AtomicExpr:

                case CX_StmtClass.CX_StmtClass_BinaryOperator:
                {
                    var binaryOperator = (BinaryOperator)initExpr;
                    return IsConstant(binaryOperator.LHS) && IsConstant(binaryOperator.RHS);
                }

                // case CX_StmtClass.CX_StmtClass_CompoundAssignOperator:
                // case CX_StmtClass.CX_StmtClass_BlockExpr:
                // case CX_StmtClass.CX_StmtClass_CXXBindTemporaryExpr:

                case CX_StmtClass.CX_StmtClass_CXXBoolLiteralExpr:
                {
                    return true;
                }

                // case CX_StmtClass.CX_StmtClass_CXXConstructExpr:
                // case CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultArgExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDeleteExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDependentScopeMemberExpr:
                // case CX_StmtClass.CX_StmtClass_CXXFoldExpr:
                // case CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXNewExpr:
                // case CX_StmtClass.CX_StmtClass_CXXNoexceptExpr:

                case CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr:
                {
                    return true;
                }

                // case CX_StmtClass.CX_StmtClass_CXXPseudoDestructorExpr:
                // case CX_StmtClass.CX_StmtClass_CXXRewrittenBinaryOperator:
                // case CX_StmtClass.CX_StmtClass_CXXScalarValueInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXStdInitializerListExpr:
                // case CX_StmtClass.CX_StmtClass_CXXThisExpr:
                // case CX_StmtClass.CX_StmtClass_CXXThrowExpr:
                // case CX_StmtClass.CX_StmtClass_CXXTypeidExpr:
                // case CX_StmtClass.CX_StmtClass_CXXUnresolvedConstructExpr:
                // case CX_StmtClass.CX_StmtClass_CXXUuidofExpr:

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
                {
                    var cStyleCastExpr = (CStyleCastExpr)initExpr;
                    return IsConstant(cStyleCastExpr.SubExprAsWritten);
                }

                // case CX_StmtClass.CX_StmtClass_CXXFunctionalCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXConstCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr:

                case CX_StmtClass.CX_StmtClass_CXXStaticCastExpr:
                {
                    var cxxStaticCastExpr = (CXXStaticCastExpr)initExpr;
                    return IsConstant(cxxStaticCastExpr.SubExprAsWritten);
                }

                // case CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr:

                case CX_StmtClass.CX_StmtClass_ImplicitCastExpr:
                {
                    var implicitCastExpr = (ImplicitCastExpr)initExpr;
                    return IsConstant(implicitCastExpr.SubExprAsWritten);
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
                           ((declRefExpr.Decl is VarDecl varDecl) && varDecl.HasInit && IsConstant(varDecl.Init));
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
                // case CX_StmtClass.CX_StmtClass_ExprWithCleanups:
                // case CX_StmtClass.CX_StmtClass_FunctionParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_GNUNullExpr:
                // case CX_StmtClass.CX_StmtClass_GenericSelectionExpr:
                // case CX_StmtClass.CX_StmtClass_ImaginaryLiteral:
                // case CX_StmtClass.CX_StmtClass_ImplicitValueInitExpr:
                // case CX_StmtClass.CX_StmtClass_InitListExpr:

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
                // case CX_StmtClass.CX_StmtClass_OffsetOfExpr:
                // case CX_StmtClass.CX_StmtClass_OpaqueValueExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedLookupExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr:
                // case CX_StmtClass.CX_StmtClass_PackExpansionExpr:

                case CX_StmtClass.CX_StmtClass_ParenExpr:
                {
                    var parenExpr = (ParenExpr)initExpr;
                    return IsConstant(parenExpr.SubExpr);
                }

                // case CX_StmtClass.CX_StmtClass_ParenListExpr:
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

                    long size32;
                    long size64;

                    long alignment32 = -1;
                    long alignment64 = -1;

                    GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out size32, out size64);

                    switch (unaryExprOrTypeTraitExpr.Kind)
                    {
                        case CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf:
                        {
                            return (size32 == size64);
                        }

                        case CX_UnaryExprOrTypeTrait.CX_UETT_AlignOf:
                        case CX_UnaryExprOrTypeTrait.CX_UETT_PreferredAlignOf:
                        {
                            return (alignment32 == alignment64);
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
                    return IsConstant(unaryOperator.SubExpr);
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
