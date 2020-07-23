// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    // We don't currently include the namespace name anywhere in the
                    // generated bindings. We might want to in the future...
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
            var name = GetRemappedCursorName(enumConstantDecl);

            _outputBuilder.WriteIndentation();
            _outputBuilder.Write(EscapeName(name));

            if (enumConstantDecl.InitExpr != null)
            {
                _outputBuilder.Write(' ');
                _outputBuilder.Write('=');
                _outputBuilder.Write(' ');
                Visit(enumConstantDecl.InitExpr);
            }

            _outputBuilder.WriteLine(',');
        }

        private void VisitEnumDecl(EnumDecl enumDecl)
        {
            var name = GetRemappedCursorName(enumDecl);

            StartUsingOutputBuilder(name);
            {
                var integerTypeName = GetRemappedTypeName(enumDecl, context: null, enumDecl.IntegerType, out var nativeTypeName);

                WithType("*", ref integerTypeName, ref nativeTypeName);
                WithType(name, ref integerTypeName, ref nativeTypeName);

                AddNativeTypeNameAttribute(nativeTypeName);

                _outputBuilder.WriteIndented(GetAccessSpecifierName(enumDecl));
                _outputBuilder.Write(' ');
                _outputBuilder.Write("enum");
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeName(name));

                if (!integerTypeName.Equals("int"))
                {
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(':');
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(integerTypeName);
                }

                _outputBuilder.NeedsNewline = true;
                _outputBuilder.WriteBlockStart();

                Visit(enumDecl.Enumerators);
                Visit(enumDecl.Decls, excludedCursors: enumDecl.Enumerators);

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitFieldDecl(FieldDecl fieldDecl)
        {
            if (fieldDecl.IsBitField)
            {
                return;
            }

            var name = GetRemappedCursorName(fieldDecl);

            if (name.StartsWith("__AnonymousField_"))
            {
                var newName = "Anonymous";

                if (fieldDecl.Parent.AnonymousDecls.Count != 1)
                {
                    var index = fieldDecl.Parent.AnonymousDecls.IndexOf(fieldDecl) + 1;
                    newName += index.ToString();
                }

                var remappedNames = _config.RemappedNames as Dictionary<string, string>;
                remappedNames.Add(name, newName);

                name = newName;
            }

            var escapedName = EscapeName(name);

            var type = fieldDecl.Type;
            var typeName = GetRemappedTypeName(fieldDecl, context: null, type, out var nativeTypeName);

            if (typeName.StartsWith("__AnonymousRecord_"))
            {
                var newTypeName = $"_{name}_e__{(((type.CanonicalType is RecordType recordType) && recordType.Decl.IsUnion) ? "Union" : "Struct")}";

                var remappedNames = _config.RemappedNames as Dictionary<string, string>;
                remappedNames.Add(typeName, newTypeName);

                typeName = newTypeName;
            }

            if (fieldDecl.Parent.IsUnion)
            {
                _outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
            }
            AddNativeTypeNameAttribute(nativeTypeName);

            _outputBuilder.WriteIndented(GetAccessSpecifierName(fieldDecl));
            _outputBuilder.Write(' ');

            if (NeedsNewKeyword(name))
            {
                _outputBuilder.Write("new");
                _outputBuilder.Write(' ');
            }

            if (type is ConstantArrayType constantArrayType)
            {
                if (IsSupportedFixedSizedBufferType(typeName))
                {
                    _outputBuilder.Write("fixed");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                    _outputBuilder.Write('[');
                    _outputBuilder.Write(Math.Max(constantArrayType.Size, 1));

                    var elementType = constantArrayType.ElementType;

                    while (elementType is ConstantArrayType subConstantArrayType)
                    {
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write('*');
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(subConstantArrayType.Size);

                        elementType = subConstantArrayType.ElementType;
                    }

                    _outputBuilder.Write(']');
                }
                else
                {
                    _outputBuilder.Write(GetArtificalFixedSizedBufferName(fieldDecl));
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
            var functionType = (FunctionType)type;

            if (callConv == CXCallingConv.CXCallingConv_Invalid)
            {
                callConv = functionType.CallConv;
            }

            var cxxMethodDecl = functionDecl as CXXMethodDecl;
            var body = functionDecl.Body;
            var isVirtual = (cxxMethodDecl != null) && cxxMethodDecl.IsVirtual;

            if (isVirtual)
            {
                Debug.Assert(!_config.GeneratePreviewCodeFnptr);

                _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                _outputBuilder.WriteIndented("[UnmanagedFunctionPointer");

                var callingConventionName = GetCallingConventionName(functionDecl, callConv, name);

                _outputBuilder.Write('(');
                _outputBuilder.Write("CallingConvention");
                _outputBuilder.Write('.');
                _outputBuilder.Write(callingConventionName);
                _outputBuilder.Write(')');

                _outputBuilder.WriteLine(']');
            }
            else if (body is null)
            {
                _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                _outputBuilder.WriteIndented("[DllImport(");

                WithLibraryPath(name);

                _outputBuilder.Write(',');
                _outputBuilder.Write(' ');

                var callingConventionName = GetCallingConventionName(functionDecl, callConv, name);

                if (callingConventionName != "Winapi")
                {
                    _outputBuilder.Write("CallingConvention");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write('=');
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write("CallingConvention");
                    _outputBuilder.Write('.');
                    _outputBuilder.Write(callingConventionName);
                    _outputBuilder.Write(',');
                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write("EntryPoint = \"");

                if (cxxMethodDecl is null)
                {
                    _outputBuilder.Write(name);
                }
                else
                {
                    _outputBuilder.Write(cxxMethodDecl.Handle.Mangling);
                }

                _outputBuilder.Write("\", ExactSpelling = true");
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

            _outputBuilder.WriteIndented(GetAccessSpecifierName(functionDecl));

            if (isVirtual)
            {
                _outputBuilder.Write(' ');
                _outputBuilder.Write("delegate");
            }
            else if ((body is null) || (cxxMethodDecl is null) || cxxMethodDecl.IsStatic)
            {
                _outputBuilder.Write(' ');
                _outputBuilder.Write("static");

                if (body is null)
                {
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write("extern");
                }
            }

            _outputBuilder.Write(' ');

            if (!isVirtual)
            {
                if (NeedsNewKeyword(name, functionDecl.Parameters))
                {
                    _outputBuilder.Write("new");
                    _outputBuilder.Write(' ');
                }

                if (IsUnsafe(functionDecl))
                {
                    if (cxxRecordDecl is null)
                    {
                        _isMethodClassUnsafe = true;
                    }
                    else if (!IsUnsafe(cxxRecordDecl))
                    {
                        _outputBuilder.Write("unsafe");
                        _outputBuilder.Write(' ');
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

            if (isVirtual)
            {
                _outputBuilder.Write(PrefixAndStripName(name));
            }
            else
            {
                _outputBuilder.Write(EscapeAndStripName(name));
            }

            _outputBuilder.Write('(');

            if (isVirtual)
            {
                Debug.Assert(cxxRecordDecl != null);

                var previousContext = PreviousContext;

                if (!(previousContext is CXXRecordDecl thisCursor))
                {
                    thisCursor = cxxRecordDecl;
                }

                var cxxRecordDeclName = GetRemappedCursorName(thisCursor);
                _outputBuilder.Write(EscapeName(cxxRecordDeclName));
                _outputBuilder.Write('*');

                _outputBuilder.Write(' ');
                _outputBuilder.Write("pThis");

                if (needsReturnFixup)
                {
                    _outputBuilder.Write(',');
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(returnTypeName);
                    _outputBuilder.Write('*');
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write("_result");
                }

                if (functionDecl.Parameters.Any())
                {
                    _outputBuilder.Write(',');
                    _outputBuilder.Write(' ');
                }
            }

            Visit(functionDecl.Parameters);

            _outputBuilder.Write(")");

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
                    VisitCtorInitializers(this, cxxConstructorDecl, firstCtorInitializer, lastCtorInitializer);
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

            static void VisitCtorInitializers(PInvokeGenerator pinvokeGenerator, CXXConstructorDecl cxxConstructorDecl, int firstCtorInitializer, int lastCtorInitializer)
            {
                var outputBuilder = pinvokeGenerator._outputBuilder;

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

                    var memberRefName = pinvokeGenerator.GetRemappedCursorName(memberRef.Referenced);
                    var memberInitName = memberInit.Spelling;

                    if ((memberInit is ImplicitCastExpr implicitCastExpr) && (implicitCastExpr.SubExpr is DeclRefExpr declRefExpr))
                    {
                        memberInitName = pinvokeGenerator.GetRemappedCursorName(declRefExpr.Decl);
                    }
                    outputBuilder.WriteIndentation();

                    if (memberRefName.Equals(memberInitName))
                    {
                        outputBuilder.Write("this");
                        outputBuilder.Write('.');
                    }

                    pinvokeGenerator.Visit(memberRef);
                    outputBuilder.Write(' ');
                    outputBuilder.Write('=');
                    outputBuilder.Write(' ');
                    pinvokeGenerator.Visit(memberInit);
                    outputBuilder.WriteSemicolon();
                    outputBuilder.WriteNewline();
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

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl)
        {
            if (IsExcluded(parmVarDecl))
            {
                return;
            }

            var previousContext = PreviousContext;

            if (previousContext is FunctionDecl functionDecl)
            {
                ForFunctionDecl(parmVarDecl, functionDecl);
            }
            else if (previousContext is TypedefDecl typedefDecl)
            {
                ForTypedefDecl(parmVarDecl, typedefDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported parameter variable declaration parent: '{previousContext.CursorKindSpelling}'. Generated bindings may be incomplete.", previousContext);
            }

            void ForFunctionDecl(ParmVarDecl parmVarDecl, FunctionDecl functionDecl)
            {
                var type = parmVarDecl.Type;
                var typeName = GetRemappedTypeName(parmVarDecl, context: null, type, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName, prefix: "", postfix: " ");

                if ((((functionDecl is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsVirtual) || (functionDecl.Body is null)) && (typeName == "bool"))
                {
                    // bool is not blittable, so we shouldn't use it for P/Invoke signatures
                    typeName = "byte";
                    nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? "bool" : nativeTypeName;
                }

                _outputBuilder.Write(typeName);

                if (type is ArrayType)
                {
                    _outputBuilder.Write('*');
                }

                _outputBuilder.Write(' ');

                var name = GetRemappedCursorName(parmVarDecl);
                _outputBuilder.Write(EscapeName(name));

                var parameters = functionDecl.Parameters;
                var index = parameters.IndexOf(parmVarDecl);
                var lastIndex = parameters.Count - 1;

                if (name.Equals("param"))
                {
                    _outputBuilder.Write(index);
                }

                if (parmVarDecl.HasDefaultArg)
                {
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write('=');
                    _outputBuilder.Write(' ');

                    var defaultArg = parmVarDecl.DefaultArg;

                    if ((defaultArg is UnaryExprOrTypeTraitExpr unaryExprOrTypeTraitExpr) &&
                        (unaryExprOrTypeTraitExpr.Kind == CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf) &&
                        IsFixedSize(unaryExprOrTypeTraitExpr, unaryExprOrTypeTraitExpr.TypeOfArgument))
                    {
                        _outputBuilder.Write(unaryExprOrTypeTraitExpr.TypeOfArgument.Handle.SizeOf);
                    }
                    else
                    {
                        Visit(defaultArg);
                    }
                }

                if (index != lastIndex)
                {
                    _outputBuilder.Write(',');
                    _outputBuilder.Write(' ');
                }
            }

            void ForTypedefDecl(ParmVarDecl parmVarDecl, TypedefDecl typedefDecl)
            {
                var type = parmVarDecl.Type;
                var typeName = GetRemappedTypeName(parmVarDecl, context: null, type, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName, prefix: "", postfix: " ");

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
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write('=');
                    _outputBuilder.Write(' ');
                    Visit(parmVarDecl.DefaultArg);
                }

                if (index != lastIndex)
                {
                    _outputBuilder.Write(',');
                    _outputBuilder.Write(' ');
                }
            }
        }

        private void VisitRecordDecl(RecordDecl recordDecl)
        {
            var name = GetRemappedCursorName(recordDecl);

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
                    _testOutputBuilder.WriteIndented("/// <summary>Provides validation of the <see cref=");
                    _testOutputBuilder.Write('"');
                    _testOutputBuilder.Write(EscapeName(name));
                    _testOutputBuilder.Write('"');
                    _testOutputBuilder.WriteLine(" /> struct.</summary>");
                    _testOutputBuilder.WriteIndented("public static unsafe class");
                    _testOutputBuilder.Write(' ');
                    _testOutputBuilder.Write(EscapeName(name));
                    _testOutputBuilder.WriteLine("Tests");
                    _testOutputBuilder.WriteBlockStart();
                }

                if (recordDecl.IsUnion)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    _outputBuilder.WriteIndented("[StructLayout(LayoutKind.Explicit");

                    if (alignment < maxAlignm)
                    {
                        _outputBuilder.Write(',');
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write("Pack");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write('=');
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(alignment);
                    }
                    _outputBuilder.Write(')');
                    _outputBuilder.WriteLine(']');
                }
                else if (alignment < maxAlignm)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    _outputBuilder.WriteIndented("[StructLayout(LayoutKind.Sequential");

                    _outputBuilder.Write(',');
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write("Pack");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write('=');
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(alignment);
                    _outputBuilder.Write(')');
                    _outputBuilder.WriteLine(']');
                }

                _outputBuilder.WriteIndented(GetAccessSpecifierName(recordDecl));
                _outputBuilder.Write(' ');

                if (IsUnsafe(recordDecl))
                {
                    _outputBuilder.Write("unsafe");
                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write("partial struct");
                _outputBuilder.Write(' ');
                _outputBuilder.WriteLine(EscapeName(name));
                _outputBuilder.WriteBlockStart();

                if (hasVtbl)
                {
                    if (_config.GenerateExplicitVtbls)
                    {
                        _outputBuilder.WriteIndented("public");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write("Vtbl");
                        _outputBuilder.Write('*');
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write("lpVtbl");
                        _outputBuilder.WriteSemicolon();
                        _outputBuilder.WriteNewline();
                    }
                    else
                    {
                        _outputBuilder.WriteIndented("public");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write("void");
                        _outputBuilder.Write('*');
                        _outputBuilder.Write('*');
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write("lpVtbl");
                        _outputBuilder.WriteSemicolon();
                        _outputBuilder.WriteNewline();
                    }
                    _outputBuilder.NeedsNewline = true;

                    if ((_testOutputBuilder != null) && !recordDecl.IsAnonymousStructOrUnion && !(recordDecl.DeclContext is RecordDecl))
                    {
                        _testOutputBuilder.AddUsingDirective($"static {_config.Namespace}.{_config.MethodClassName}");

                        _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"Guid\" /> of the <see cref=");
                        _testOutputBuilder.Write('"');
                        _testOutputBuilder.Write(EscapeName(name));
                        _testOutputBuilder.Write('"');
                        _testOutputBuilder.WriteLine(" /> struct is correct.</summary>");

                        WithTestAttribute();

                        _testOutputBuilder.WriteIndented("public static void");
                        _testOutputBuilder.Write(' ');
                        _testOutputBuilder.Write("GuidOfTest");
                        _testOutputBuilder.Write('(');
                        _testOutputBuilder.WriteLine(')');
                        _testOutputBuilder.WriteBlockStart();

                        if (_config.GenerateTestsNUnit)
                        {
                            _testOutputBuilder.WriteIndented("Assert.That");
                        }
                        else if (_config.GenerateTestsXUnit)
                        {
                            _testOutputBuilder.WriteIndented("Assert.Equal");
                        }

                        _testOutputBuilder.Write('(');
                        _testOutputBuilder.Write("typeof");
                        _testOutputBuilder.Write('(');
                        _testOutputBuilder.Write(EscapeName(name));
                        _testOutputBuilder.Write(')');
                        _testOutputBuilder.Write('.');
                        _testOutputBuilder.Write("GUID");
                        _testOutputBuilder.Write(',');
                        _testOutputBuilder.Write(' ');

                        if (_config.GenerateTestsNUnit)
                        {
                            _testOutputBuilder.Write("Is");
                            _testOutputBuilder.Write('.');
                            _testOutputBuilder.Write("EqualTo");
                            _testOutputBuilder.Write('(');
                        }

                        _testOutputBuilder.Write("IID");
                        _testOutputBuilder.Write('_');
                        _testOutputBuilder.Write(EscapeName(name));

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

                if (cxxRecordDecl != null)
                {
                    foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                    {
                        var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);

                        if (HasFields(this, baseCxxRecordDecl))
                        {
                            _outputBuilder.WriteIndented(GetAccessSpecifierName(baseCxxRecordDecl));
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write(GetRemappedCursorName(baseCxxRecordDecl));
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write(GetRemappedAnonymousName(cxxBaseSpecifier, "Base"));
                            _outputBuilder.WriteSemicolon();
                            _outputBuilder.WriteNewline();

                            _outputBuilder.NeedsNewline = true;
                        }
                    }
                }

                if ((_testOutputBuilder != null) && !recordDecl.IsAnonymousStructOrUnion && !(recordDecl.DeclContext is RecordDecl))
                {
                    _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=");
                    _testOutputBuilder.Write('"');
                    _testOutputBuilder.Write(EscapeName(name));
                    _testOutputBuilder.Write('"');
                    _testOutputBuilder.WriteLine(" /> struct is blittable.</summary>");

                    WithTestAttribute();

                    _testOutputBuilder.WriteIndented("public static void");
                    _testOutputBuilder.Write(' ');
                    _testOutputBuilder.Write("IsBlittableTest");
                    _testOutputBuilder.Write('(');
                    _testOutputBuilder.WriteLine(')');
                    _testOutputBuilder.WriteBlockStart();

                    WithTestAssertEqual($"sizeof({EscapeName(name)})", $"Marshal.SizeOf<{EscapeName(name)}>()");

                    _testOutputBuilder.WriteBlockEnd();
                    _testOutputBuilder.NeedsNewline = true;

                    _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=");
                    _testOutputBuilder.Write('"');
                    _testOutputBuilder.Write(EscapeName(name));
                    _testOutputBuilder.Write('"');
                    _testOutputBuilder.WriteLine(" /> struct has the right <see cref=\"LayoutKind\" />.</summary>");

                    WithTestAttribute();

                    _testOutputBuilder.WriteIndented("public static void");
                    _testOutputBuilder.Write(' ');
                    _testOutputBuilder.Write("IsLayout");

                    if (recordDecl.IsUnion)
                    {
                        _testOutputBuilder.Write("Explicit");
                    }
                    else
                    {
                        _testOutputBuilder.Write("Sequential");
                    }

                    _testOutputBuilder.Write("Test");
                    _testOutputBuilder.Write('(');
                    _testOutputBuilder.WriteLine(')');
                    _testOutputBuilder.WriteBlockStart();

                    WithTestAssertTrue($"typeof({EscapeName(name)}).Is{(recordDecl.IsUnion ? "ExplicitLayout" : "LayoutSequential")}");

                    _testOutputBuilder.WriteBlockEnd();
                    _testOutputBuilder.NeedsNewline = true;

                    long alignment32 = -1;
                    long alignment64 = -1;

                    GetTypeSize(recordDecl, recordDecl.TypeForDecl, ref alignment32, ref alignment64, out var size32, out var size64);

                    if ((size32 == 0) || (size64 == 0))
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"{EscapeName(name)} has a size of 0");
                    }

                    _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=");
                    _testOutputBuilder.Write('"');
                    _testOutputBuilder.Write(EscapeName(name));
                    _testOutputBuilder.Write('"');
                    _testOutputBuilder.WriteLine(" /> struct has the correct size.</summary>");

                    WithTestAttribute();

                    _testOutputBuilder.WriteIndented("public static void");
                    _testOutputBuilder.Write(' ');
                    _testOutputBuilder.Write("SizeOfTest");
                    _testOutputBuilder.Write('(');
                    _testOutputBuilder.WriteLine(')');
                    _testOutputBuilder.WriteBlockStart();

                    if (size32 != size64)
                    {
                        _testOutputBuilder.AddUsingDirective("System");

                        _testOutputBuilder.WriteIndented("if");
                        _testOutputBuilder.Write(' ');
                        _testOutputBuilder.Write('(');
                        _testOutputBuilder.Write("Environment");
                        _testOutputBuilder.Write('.');
                        _testOutputBuilder.Write("Is64BitProcess");
                        _testOutputBuilder.WriteLine(')');
                        _testOutputBuilder.WriteBlockStart();

                        WithTestAssertEqual($"{size64}", $"sizeof({EscapeName(name)})");

                        _testOutputBuilder.WriteBlockEnd();
                        _testOutputBuilder.WriteIndentedLine("else");
                        _testOutputBuilder.WriteBlockStart();
                    }

                    WithTestAssertEqual($"{size32}", $"sizeof({EscapeName(name)})");

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
                            VisitBitfieldDecl(this, fieldDecl, bitfieldTypes, recordDecl, contextName: "", ref bitfieldIndex, ref bitfieldPreviousSize, ref bitfieldRemainingBits);
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
                        VisitAnonymousRecordDecl(this, _outputBuilder, recordDecl, nestedRecordDecl);
                    }
                }

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
                    OutputDelegateSignatures(this, cxxRecordDecl, cxxRecordDecl, hitsPerName: new Dictionary<string, int>());
                }

                var excludedCursors = recordDecl.Fields.AsEnumerable<Cursor>();

                if (cxxRecordDecl != null)
                {
                    OutputMethods(this, cxxRecordDecl, cxxRecordDecl);
                    excludedCursors = excludedCursors.Concat(cxxRecordDecl.Methods);
                }

                Visit(recordDecl.Decls, excludedCursors);

                foreach (var constantArray in recordDecl.Fields.Where((field) => field.Type is ConstantArrayType))
                {
                    VisitConstantArrayFieldDecl(this, recordDecl, constantArray);
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
                    OutputVtblHelperMethods(this, cxxRecordDecl, cxxRecordDecl, ref index, hitsPerName: new Dictionary<string, int>());

                    if (_config.GenerateExplicitVtbls)
                    {
                        _outputBuilder.NeedsNewline = true;
                        _outputBuilder.WriteIndentedLine("public partial struct Vtbl");
                        _outputBuilder.WriteBlockStart();
                        OutputVtblEntries(this, cxxRecordDecl, cxxRecordDecl, hitsPerName: new Dictionary<string, int>());
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

            static string FixupNameForMultipleHits(PInvokeGenerator pinvokeGenerator, CXXMethodDecl cxxMethodDecl, Dictionary<string, int> hitsPerName)
            {
                var remappedName = pinvokeGenerator.GetRemappedCursorName(cxxMethodDecl);

                if (hitsPerName.TryGetValue(remappedName, out int hits))
                {
                    hitsPerName[remappedName] = (hits + 1);

                    var name = pinvokeGenerator.GetCursorName(cxxMethodDecl);
                    var remappedNames = (Dictionary<string, string>)pinvokeGenerator._config.RemappedNames;

                    remappedNames[name] = $"{remappedName}{hits}";
                }
                else
                {
                    hitsPerName.Add(remappedName, 1);
                }

                return remappedName;
            }

            static bool HasFields(PInvokeGenerator pinvokeGenerator, CXXRecordDecl cxxRecordDecl)
            {
                var hasFields = cxxRecordDecl.Fields.Any();

                if (!hasFields)
                {
                    foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                    {
                        var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);

                        if (HasFields(pinvokeGenerator, baseCxxRecordDecl))
                        {
                            hasFields = true;
                            break;
                        }
                    }
                }
                return hasFields;
            }

            static void OutputDelegateSignatures(PInvokeGenerator pinvokeGenerator, CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, Dictionary<string, int> hitsPerName)
            {
                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);
                    OutputDelegateSignatures(pinvokeGenerator, rootCxxRecordDecl, baseCxxRecordDecl, hitsPerName);
                }

                foreach (var cxxMethodDecl in cxxRecordDecl.Methods)
                {
                    if (!cxxMethodDecl.IsVirtual)
                    {
                        continue;
                    }

                    if (!pinvokeGenerator._config.GeneratePreviewCodeFnptr)
                    {
                        pinvokeGenerator._outputBuilder.NeedsNewline = true;

                        var remappedName = FixupNameForMultipleHits(pinvokeGenerator, cxxMethodDecl, hitsPerName);
                        Debug.Assert(pinvokeGenerator.CurrentContext == rootCxxRecordDecl);
                        pinvokeGenerator.Visit(cxxMethodDecl);
                        RestoreNameForMultipleHits(pinvokeGenerator, cxxMethodDecl, hitsPerName, remappedName);
                    }
                }
            }

            static void OutputMethods(PInvokeGenerator pinvokeGenerator, CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
            {
                var outputBuilder = pinvokeGenerator._outputBuilder;

                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);
                    OutputMethods(pinvokeGenerator, rootCxxRecordDecl, baseCxxRecordDecl);
                }

                foreach (var cxxMethodDecl in cxxRecordDecl.Methods)
                {
                    if (cxxMethodDecl.IsVirtual || (cxxMethodDecl is CXXConstructorDecl) || (cxxMethodDecl is CXXDestructorDecl))
                    {
                        continue;
                    }

                    Debug.Assert(pinvokeGenerator.CurrentContext == rootCxxRecordDecl);
                    pinvokeGenerator.Visit(cxxMethodDecl);
                    outputBuilder.NeedsNewline = true;
                }
            }

            static void OutputVtblEntries(PInvokeGenerator pinvokeGenerator, CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, Dictionary<string, int> hitsPerName)
            {
                var outputBuilder = pinvokeGenerator._outputBuilder;

                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);
                    OutputVtblEntries(pinvokeGenerator, rootCxxRecordDecl, baseCxxRecordDecl, hitsPerName);
                }

                var cxxMethodDecls = cxxRecordDecl.Methods;

                if (cxxMethodDecls.Count != 0)
                {
                    foreach (var cxxMethodDecl in cxxMethodDecls)
                    {
                        OutputVtblEntry(pinvokeGenerator, outputBuilder, rootCxxRecordDecl, cxxMethodDecl, hitsPerName);
                        outputBuilder.NeedsNewline = true;
                    }
                }
            }

            static void OutputVtblEntry(PInvokeGenerator pinvokeGenerator, OutputBuilder outputBuilder, CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl, Dictionary<string, int> hitsPerName)
            {
                if (!cxxMethodDecl.IsVirtual || pinvokeGenerator.IsExcluded(cxxMethodDecl))
                {
                    return;
                }

                var cxxMethodDeclTypeName = pinvokeGenerator.GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, cxxMethodDecl.Type, out var nativeTypeName);
                pinvokeGenerator.AddNativeTypeNameAttribute(nativeTypeName);

                outputBuilder.WriteIndented(pinvokeGenerator.GetAccessSpecifierName(cxxMethodDecl));
                outputBuilder.Write(' ');

                var remappedName = FixupNameForMultipleHits(pinvokeGenerator, cxxMethodDecl, hitsPerName);
                var cxxMethodDeclName = pinvokeGenerator.GetRemappedCursorName(cxxMethodDecl);
                RestoreNameForMultipleHits(pinvokeGenerator, cxxMethodDecl, hitsPerName, remappedName);

                var escapedName = pinvokeGenerator.EscapeAndStripName(cxxMethodDeclName);

                if (pinvokeGenerator.NeedsNewKeyword(escapedName))
                {
                    outputBuilder.Write("new");
                    outputBuilder.Write(' ');
                }

                outputBuilder.Write(cxxMethodDeclTypeName);
                outputBuilder.Write(' ');

                outputBuilder.Write(escapedName);

                outputBuilder.WriteSemicolon();
                outputBuilder.WriteNewline();
            }

            static void OutputVtblHelperMethod(PInvokeGenerator pinvokeGenerator, OutputBuilder outputBuilder, CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl, ref int vtblIndex, Dictionary<string, int> hitsPerName)
            {
                if (!cxxMethodDecl.IsVirtual || pinvokeGenerator.IsExcluded(cxxMethodDecl))
                {
                    return;
                }

                var currentContext = pinvokeGenerator._context.AddLast(cxxMethodDecl);

                var returnType = cxxMethodDecl.ReturnType;
                var returnTypeName = pinvokeGenerator.GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, returnType, out var nativeTypeName);
                pinvokeGenerator.AddNativeTypeNameAttribute(nativeTypeName, attributePrefix: "return: ");

                outputBuilder.WriteIndented(pinvokeGenerator.GetAccessSpecifierName(cxxMethodDecl));
                outputBuilder.Write(' ');

                var remappedName = FixupNameForMultipleHits(pinvokeGenerator, cxxMethodDecl, hitsPerName);
                var cxxMethodDeclName = pinvokeGenerator.GetRemappedCursorName(cxxMethodDecl);
                RestoreNameForMultipleHits(pinvokeGenerator, cxxMethodDecl, hitsPerName, remappedName);

                var escapedName = pinvokeGenerator.EscapeAndStripName(remappedName);

                if (pinvokeGenerator.NeedsNewKeyword(escapedName, cxxMethodDecl.Parameters))
                {
                    outputBuilder.Write("new");
                    outputBuilder.Write(' ');
                }

                outputBuilder.Write(returnTypeName);
                outputBuilder.Write(' ');

                outputBuilder.Write(escapedName);

                outputBuilder.Write('(');

                pinvokeGenerator.Visit(cxxMethodDecl.Parameters);

                outputBuilder.WriteLine(')');
                outputBuilder.WriteBlockStart();

                var needsReturnFixup = false;
                var cxxRecordDeclName = pinvokeGenerator.GetRemappedCursorName(cxxRecordDecl);
                var escapedCXXRecordDeclName = pinvokeGenerator.EscapeName(cxxRecordDeclName);

                outputBuilder.WriteIndentation();

                if (pinvokeGenerator._config.GenerateCompatibleCode)
                {
                    outputBuilder.Write("fixed");
                    outputBuilder.Write(' ');
                    outputBuilder.Write('(');
                    outputBuilder.Write(escapedCXXRecordDeclName);
                    outputBuilder.Write('*');
                    outputBuilder.Write(' ');
                    outputBuilder.Write("pThis");
                    outputBuilder.Write(' ');
                    outputBuilder.Write('=');
                    outputBuilder.Write(' ');
                    outputBuilder.Write('&');
                    outputBuilder.Write("this");
                    outputBuilder.WriteLine(')');
                    outputBuilder.WriteBlockStart();
                    outputBuilder.WriteIndentation();
                }

                if (returnType.Kind != CXTypeKind.CXType_Void)
                {
                    needsReturnFixup = pinvokeGenerator.NeedsReturnFixup(cxxMethodDecl);

                    if (needsReturnFixup)
                    {
                        outputBuilder.Write(returnTypeName);
                        outputBuilder.Write(' ');
                        outputBuilder.Write("result");
                        outputBuilder.WriteSemicolon();
                        outputBuilder.WriteNewline();
                        outputBuilder.WriteIndentation();
                    }

                    outputBuilder.Write("return");
                    outputBuilder.Write(' ');
                }

                if (needsReturnFixup)
                {
                    outputBuilder.Write('*');
                }

                if (!pinvokeGenerator._config.GeneratePreviewCodeFnptr)
                {
                    outputBuilder.Write("Marshal.GetDelegateForFunctionPointer<");
                    outputBuilder.Write(pinvokeGenerator.PrefixAndStripName(cxxMethodDeclName));
                    outputBuilder.Write(">");
                    outputBuilder.Write('(');
                }

                if (pinvokeGenerator._config.GenerateExplicitVtbls)
                {
                    outputBuilder.Write("lpVtbl->");
                    outputBuilder.Write(pinvokeGenerator.EscapeAndStripName(cxxMethodDeclName));
                }
                else
                {
                    var cxxMethodDeclTypeName = pinvokeGenerator.GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, cxxMethodDecl.Type, out var _);

                    if (pinvokeGenerator._config.GeneratePreviewCodeFnptr)
                    {
                        outputBuilder.Write('(');
                    }

                    outputBuilder.Write('(');
                    outputBuilder.Write(cxxMethodDeclTypeName);
                    outputBuilder.Write(')');
                    outputBuilder.Write('(');
                    outputBuilder.Write("lpVtbl");
                    outputBuilder.Write('[');
                    outputBuilder.Write(vtblIndex);
                    outputBuilder.Write(']');
                    outputBuilder.Write(')');

                    if (pinvokeGenerator._config.GeneratePreviewCodeFnptr)
                    {
                        outputBuilder.Write(')');
                    }
                }

                if (!pinvokeGenerator._config.GeneratePreviewCodeFnptr)
                {
                    outputBuilder.Write(')');
                }

                outputBuilder.Write('(');

                if (pinvokeGenerator._config.GenerateCompatibleCode)
                {
                    outputBuilder.Write("pThis");
                }
                else
                {
                    outputBuilder.Write('(');
                    outputBuilder.Write(escapedCXXRecordDeclName);
                    outputBuilder.Write('*');
                    outputBuilder.Write(')');
                    outputBuilder.Write("Unsafe.AsPointer(ref this)");
                }

                if (needsReturnFixup)
                {
                    outputBuilder.Write(',');
                    outputBuilder.Write(' ');
                    outputBuilder.Write("&result");
                }

                var parmVarDecls = cxxMethodDecl.Parameters;

                for (int index = 0; index < parmVarDecls.Count; index++)
                {
                    outputBuilder.Write(',');
                    outputBuilder.Write(' ');

                    var name = pinvokeGenerator.GetRemappedCursorName(parmVarDecls[index]);
                    outputBuilder.Write(pinvokeGenerator.EscapeName(name));

                    if (name.Equals("param"))
                    {
                        outputBuilder.Write(index);
                    }
                }

                outputBuilder.Write(')');

                if (returnTypeName == "bool")
                {
                    outputBuilder.Write(' ');
                    outputBuilder.Write('!');
                    outputBuilder.Write('=');
                    outputBuilder.Write(' ');
                    outputBuilder.Write('0');
                }

                outputBuilder.WriteSemicolon();
                outputBuilder.WriteNewline();

                if (pinvokeGenerator._config.GenerateCompatibleCode)
                {
                    outputBuilder.WriteBlockEnd();
                }

                outputBuilder.WriteBlockEnd();
                vtblIndex += 1;

                Debug.Assert(pinvokeGenerator._context.Last == currentContext);
                pinvokeGenerator._context.RemoveLast();
            }

            static void OutputVtblHelperMethods(PInvokeGenerator pinvokeGenerator, CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, ref int index, Dictionary<string, int> hitsPerName)
            {
                foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                {
                    var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);
                    OutputVtblHelperMethods(pinvokeGenerator, rootCxxRecordDecl, baseCxxRecordDecl, ref index, hitsPerName);
                }

                var cxxMethodDecls = cxxRecordDecl.Methods;
                var outputBuilder = pinvokeGenerator._outputBuilder;

                foreach (var cxxMethodDecl in cxxMethodDecls)
                {
                    outputBuilder.NeedsNewline = true;
                    OutputVtblHelperMethod(pinvokeGenerator, outputBuilder, rootCxxRecordDecl, cxxMethodDecl, ref index, hitsPerName);
                }
            }

            static void RestoreNameForMultipleHits(PInvokeGenerator pinvokeGenerator, CXXMethodDecl cxxMethodDecl, Dictionary<string, int> hitsPerName, string remappedName)
            {
                if (hitsPerName[remappedName] != 1)
                {
                    var name = pinvokeGenerator.GetCursorName(cxxMethodDecl);
                    var remappedNames = (Dictionary<string, string>)pinvokeGenerator._config.RemappedNames;

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

            static void VisitAnonymousRecordDecl(PInvokeGenerator pinvokeGenerator, OutputBuilder outputBuilder, RecordDecl recordDecl, RecordDecl nestedRecordDecl)
            {
                var nestedRecordDeclFieldName = pinvokeGenerator.GetRemappedAnonymousName(nestedRecordDecl, "Field");

                if (nestedRecordDeclFieldName.StartsWith("__AnonymousField_"))
                {
                    var newNestedRecordDeclFieldName = "Anonymous";

                    if (recordDecl.AnonymousDecls.Count != 1)
                    {
                        var index = recordDecl.AnonymousDecls.IndexOf(nestedRecordDecl) + 1;
                        newNestedRecordDeclFieldName += index.ToString();
                    }

                    var remappedNames = pinvokeGenerator._config.RemappedNames as Dictionary<string, string>;
                    remappedNames.Add(nestedRecordDeclFieldName, newNestedRecordDeclFieldName);

                    nestedRecordDeclFieldName = newNestedRecordDeclFieldName;
                }

                var nestedRecordDeclName = pinvokeGenerator.GetRemappedTypeName(nestedRecordDecl, context: null, nestedRecordDecl.TypeForDecl, out string nativeTypeName);

                if (nestedRecordDeclName.StartsWith("__AnonymousRecord_"))
                {
                    var newNestedRecordDeclName = $"_{nestedRecordDeclFieldName}_e__{(nestedRecordDecl.IsUnion ? "Union" : "Struct")}";

                    var remappedNames = pinvokeGenerator._config.RemappedNames as Dictionary<string, string>;
                    remappedNames.Add(nestedRecordDeclName, newNestedRecordDeclName);

                    nestedRecordDeclName = newNestedRecordDeclName;
                }

                if (recordDecl.IsUnion)
                {
                    outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
                }
                pinvokeGenerator.AddNativeTypeNameAttribute(nativeTypeName);

                outputBuilder.WriteIndented("internal");
                outputBuilder.Write(' ');
                outputBuilder.Write(nestedRecordDeclName);
                outputBuilder.Write(' ');
                outputBuilder.Write(nestedRecordDeclFieldName);
                outputBuilder.WriteSemicolon();
                outputBuilder.WriteNewline();
                outputBuilder.NeedsNewline = true;

                if (!recordDecl.IsAnonymousStructOrUnion)
                {
                    VisitAnonymousRecordDeclFields(pinvokeGenerator, outputBuilder, recordDecl, recordDecl, nestedRecordDecl, nestedRecordDeclName, nestedRecordDeclFieldName);
                }
            }

            static void VisitAnonymousRecordDeclFields(PInvokeGenerator pinvokeGenerator, OutputBuilder outputBuilder, RecordDecl rootRecordDecl, RecordDecl parentRecordDecl, RecordDecl anonymousRecordDecl, string contextType, string contextName)
            {
                var bitfieldTypes = pinvokeGenerator.GetBitfieldCount(anonymousRecordDecl);
                var bitfieldIndex = (bitfieldTypes.Length == 1) ? -1 : 0;

                var bitfieldPreviousSize = 0L;
                var bitfieldRemainingBits = 0L;

                foreach (var declaration in anonymousRecordDecl.Decls)
                {
                    if (declaration is FieldDecl fieldDecl)
                    {
                        if (fieldDecl.IsBitField)
                        {
                            VisitBitfieldDecl(pinvokeGenerator, fieldDecl, bitfieldTypes, rootRecordDecl, contextName, ref bitfieldIndex, ref bitfieldPreviousSize, ref bitfieldRemainingBits);
                        }
                        else
                        {
                            var type = fieldDecl.Type;
                            var typeName = pinvokeGenerator.GetRemappedTypeName(fieldDecl, context: null, type, out var fieldNativeTypeName);

                            var fieldName = pinvokeGenerator.GetRemappedCursorName(fieldDecl);

                            if (fieldName.StartsWith("__AnonymousField_"))
                            {
                                var newName = "Anonymous";

                                if (fieldDecl.Parent.AnonymousDecls.Count != 1)
                                {
                                    var index = fieldDecl.Parent.AnonymousDecls.IndexOf(fieldDecl) + 1;
                                    newName += index.ToString();
                                }

                                var remappedNames = pinvokeGenerator._config.RemappedNames as Dictionary<string, string>;
                                remappedNames.Add(fieldName, newName);

                                fieldName = newName;
                            }

                            outputBuilder.WriteIndented(pinvokeGenerator.GetAccessSpecifierName(anonymousRecordDecl));
                            outputBuilder.Write(' ');
                            outputBuilder.Write("ref");
                            outputBuilder.Write(' ');

                            if ((type.CanonicalType is RecordType recordType) && string.IsNullOrWhiteSpace(recordType.Decl.Name))
                            {
                                var nestedRecordDeclName = pinvokeGenerator.GetRemappedTypeName(recordType.Decl, context: null, recordType, out string nativeTypeName);

                                if (nestedRecordDeclName.StartsWith("__AnonymousRecord_"))
                                {
                                    var newNestedRecordDeclName = $"_{fieldName}_e__{(recordType.Decl.IsUnion ? "Union" : "Struct")}";

                                    var remappedNames = pinvokeGenerator._config.RemappedNames as Dictionary<string, string>;
                                    remappedNames.Add(nestedRecordDeclName, newNestedRecordDeclName);

                                    typeName = newNestedRecordDeclName;
                                }

                                var tmpRecordDecl = (RecordDecl)recordType.Decl.DeclContext;

                                while (tmpRecordDecl != rootRecordDecl)
                                {
                                    outputBuilder.Write(pinvokeGenerator.GetRemappedCursorName(tmpRecordDecl));
                                    outputBuilder.Write('.');
                                    tmpRecordDecl = (RecordDecl)tmpRecordDecl.DeclContext;
                                }
                            }

                            outputBuilder.Write(typeName);
                            outputBuilder.Write(' ');
                            outputBuilder.Write(fieldName);

                            if (pinvokeGenerator._config.GenerateCompatibleCode)
                            {
                                outputBuilder.WriteLine("");
                                outputBuilder.WriteBlockStart();

                                outputBuilder.WriteIndentedLine("get");
                                outputBuilder.WriteBlockStart();

                                outputBuilder.WriteIndented("fixed (");
                                outputBuilder.Write(contextType);
                                outputBuilder.Write('*');
                                outputBuilder.Write(' ');
                                outputBuilder.Write("pField");
                                outputBuilder.Write(' ');
                                outputBuilder.Write('=');
                                outputBuilder.Write(' ');
                                outputBuilder.Write('&');
                                outputBuilder.Write(contextName);
                                outputBuilder.WriteLine(')');
                                outputBuilder.WriteBlockStart();
                                outputBuilder.WriteIndented("return");
                                outputBuilder.Write(' ');
                                outputBuilder.Write("ref");
                                outputBuilder.Write(' ');
                                outputBuilder.Write("pField");
                                outputBuilder.Write('-');
                                outputBuilder.Write('>');
                                outputBuilder.Write(fieldName);
                                outputBuilder.WriteSemicolon();
                                outputBuilder.WriteNewline();
                                outputBuilder.WriteBlockEnd();
                                outputBuilder.WriteBlockEnd();
                                outputBuilder.WriteBlockEnd();
                            }
                            else
                            {
                                outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                                outputBuilder.Write(' ');
                                outputBuilder.Write("=> ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref");
                                outputBuilder.Write(' ');
                                outputBuilder.Write(contextName);
                                outputBuilder.Write('.');
                                outputBuilder.Write(fieldName);
                                outputBuilder.Write(',');
                                outputBuilder.Write(' ');
                                outputBuilder.Write('1');
                                outputBuilder.Write(')');
                                outputBuilder.Write(')');
                                outputBuilder.WriteSemicolon();
                                outputBuilder.WriteNewline();
                            }
                        }

                        outputBuilder.NeedsNewline = true;
                    }
                    else if ((declaration is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion)
                    {
                        var nestedRecordDeclFieldName = pinvokeGenerator.GetRemappedAnonymousName(nestedRecordDecl, "Field");

                        if (nestedRecordDeclFieldName.StartsWith("__AnonymousField_"))
                        {
                            var newNestedRecordDeclFieldName = "Anonymous";

                            if (parentRecordDecl.AnonymousDecls.Count != 1)
                            {
                                var index = parentRecordDecl.AnonymousDecls.IndexOf(nestedRecordDecl) + 1;
                                newNestedRecordDeclFieldName += index.ToString();
                            }

                            var remappedNames = pinvokeGenerator._config.RemappedNames as Dictionary<string, string>;
                            remappedNames.Add(nestedRecordDeclFieldName, newNestedRecordDeclFieldName);

                            nestedRecordDeclFieldName = newNestedRecordDeclFieldName;
                        }

                        var nestedRecordDeclName = pinvokeGenerator.GetRemappedTypeName(nestedRecordDecl, context: null, nestedRecordDecl.TypeForDecl, out string nativeTypeName);

                        if (nestedRecordDeclName.StartsWith("__AnonymousRecord_"))
                        {
                            var newNestedRecordDeclName = $"_{nestedRecordDeclFieldName}_e__{(nestedRecordDecl.IsUnion ? "Union" : "Struct")}";

                            var remappedNames = pinvokeGenerator._config.RemappedNames as Dictionary<string, string>;
                            remappedNames.Add(nestedRecordDeclName, newNestedRecordDeclName);

                            nestedRecordDeclName = newNestedRecordDeclName;
                        }

                        VisitAnonymousRecordDeclFields(pinvokeGenerator, outputBuilder, rootRecordDecl, anonymousRecordDecl, nestedRecordDecl, nestedRecordDeclName, $"{contextName}.{nestedRecordDeclFieldName}");
                    }
                }
            }

            static void VisitBitfieldDecl(PInvokeGenerator pinvokeGenerator, FieldDecl fieldDecl, Type[] types, RecordDecl recordDecl, string contextName, ref int index, ref long previousSize, ref long remainingBits)
            {
                Debug.Assert(fieldDecl.IsBitField);

                var outputBuilder = pinvokeGenerator._outputBuilder;
                var typeName = pinvokeGenerator.GetRemappedTypeName(fieldDecl, context: null, fieldDecl.Type, out var nativeTypeName);

                if (string.IsNullOrWhiteSpace(nativeTypeName))
                {
                    nativeTypeName = typeName;
                }
                nativeTypeName += $" : {fieldDecl.BitWidthValue}";

                if (fieldDecl.Parent.IsUnion)
                {
                    outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
                }
                var currentSize = fieldDecl.Type.Handle.SizeOf;

                var bitfieldName = "_bitfield";

                if ((!pinvokeGenerator._config.GenerateUnixTypes && (currentSize != previousSize)) || (fieldDecl.BitWidthValue > remainingBits))
                {
                    if (index >= 0)
                    {
                        index++;
                        bitfieldName += index.ToString();
                    }

                    remainingBits = currentSize * 8;
                    previousSize = 0;

                    if (fieldDecl.Parent == recordDecl)
                    {
                        outputBuilder.WriteIndented("internal");
                        outputBuilder.Write(' ');
                        outputBuilder.Write(typeName);
                        outputBuilder.Write(' ');
                        outputBuilder.Write(bitfieldName);
                        outputBuilder.WriteSemicolon();
                        outputBuilder.WriteNewline();
                        outputBuilder.NeedsNewline = true;
                    }
                }
                else
                {
                    currentSize = Math.Max(previousSize, currentSize);

                    if (pinvokeGenerator._config.GenerateUnixTypes && (currentSize > previousSize))
                    {
                        remainingBits += (currentSize - previousSize) * 8;
                    }

                    if (index >= 0)
                    {
                        bitfieldName += index.ToString();
                    }
                }

                if (!recordDecl.IsAnonymousStructOrUnion)
                {
                    pinvokeGenerator.AddNativeTypeNameAttribute(nativeTypeName);

                    var bitfieldOffset = (currentSize * 8) - remainingBits;

                    var bitwidthHexStringBacking = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X");
                    var typeBacking = (index > 0) ? types[index - 1] : types[0];
                    var canonicalTypeBacking = typeBacking.CanonicalType;

                    if (canonicalTypeBacking.Kind == CXTypeKind.CXType_Enum)
                    {
                        canonicalTypeBacking = ((EnumType)canonicalTypeBacking).Decl.IntegerType.CanonicalType;
                    }

                    var typeNameBacking = pinvokeGenerator.GetRemappedTypeName(fieldDecl, context: null, typeBacking, out _);

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
                            if (pinvokeGenerator._config.GenerateUnixTypes)
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
                            if (pinvokeGenerator._config.GenerateUnixTypes)
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
                            pinvokeGenerator.AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{canonicalTypeBacking.TypeClass}'. Generated bindings may be incomplete.", fieldDecl);
                            break;
                        }
                    }

                    var bitwidthHexString = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X");
                    var canonicalType = fieldDecl.Type.CanonicalType;

                    if (canonicalType.Kind == CXTypeKind.CXType_Enum)
                    {
                        canonicalType = ((EnumType)canonicalType).Decl.IntegerType.CanonicalType;
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
                            if (pinvokeGenerator._config.GenerateUnixTypes)
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
                            if (pinvokeGenerator._config.GenerateUnixTypes)
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
                            pinvokeGenerator.AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{canonicalType.TypeClass}'. Generated bindings may be incomplete.", fieldDecl);
                            break;
                        }
                    }

                    var name = pinvokeGenerator.GetRemappedCursorName(fieldDecl);
                    var escapedName = pinvokeGenerator.EscapeName(name);

                    outputBuilder.WriteIndented(pinvokeGenerator.GetAccessSpecifierName(fieldDecl));
                    outputBuilder.Write(' ');
                    outputBuilder.Write(typeName);
                    outputBuilder.Write(' ');
                    outputBuilder.WriteLine(escapedName);
                    outputBuilder.WriteBlockStart();
                    outputBuilder.WriteIndentedLine("get");
                    outputBuilder.WriteBlockStart();
                    outputBuilder.WriteIndented("return");
                    outputBuilder.Write(' ');

                    if ((currentSize < 4) || (canonicalTypeBacking != canonicalType))
                    {
                        outputBuilder.Write('(');
                        outputBuilder.Write(typeName);
                        outputBuilder.Write(')');
                        outputBuilder.Write('(');
                    }

                    if (bitfieldOffset != 0)
                    {
                        outputBuilder.Write('(');
                    }

                    if (!string.IsNullOrWhiteSpace(contextName))
                    {
                        outputBuilder.Write(contextName);
                        outputBuilder.Write('.');
                    }
                    outputBuilder.Write(bitfieldName);

                    if (bitfieldOffset != 0)
                    {
                        outputBuilder.Write(' ');
                        outputBuilder.Write(">>");
                        outputBuilder.Write(' ');
                        outputBuilder.Write(bitfieldOffset);
                        outputBuilder.Write(')');
                    }

                    outputBuilder.Write(' ');
                    outputBuilder.Write('&');
                    outputBuilder.Write(' ');
                    outputBuilder.Write("0x");
                    outputBuilder.Write(bitwidthHexStringBacking);

                    if ((currentSize < 4) || (canonicalTypeBacking != canonicalType))
                    {
                        outputBuilder.Write(')');
                    }

                    outputBuilder.WriteSemicolon();
                    outputBuilder.WriteNewline();
                    outputBuilder.WriteBlockEnd();

                    outputBuilder.NeedsNewline = true;

                    outputBuilder.WriteIndentedLine("set");
                    outputBuilder.WriteBlockStart();
                    outputBuilder.WriteIndentation();

                    if (!string.IsNullOrWhiteSpace(contextName))
                    {
                        outputBuilder.Write(contextName);
                        outputBuilder.Write('.');
                    }
                    outputBuilder.Write(bitfieldName);

                    outputBuilder.Write(' ');
                    outputBuilder.Write('=');
                    outputBuilder.Write(' ');

                    if (currentSize < 4)
                    {
                        outputBuilder.Write('(');
                        outputBuilder.Write(typeNameBacking);
                        outputBuilder.Write(')');
                        outputBuilder.Write('(');
                    }

                    outputBuilder.Write('(');

                    if (!string.IsNullOrWhiteSpace(contextName))
                    {
                        outputBuilder.Write(contextName);
                        outputBuilder.Write('.');
                    }
                    outputBuilder.Write(bitfieldName);

                    outputBuilder.Write(' ');
                    outputBuilder.Write('&');
                    outputBuilder.Write(' ');
                    outputBuilder.Write('~');

                    if (bitfieldOffset != 0)
                    {
                        outputBuilder.Write('(');
                    }

                    outputBuilder.Write("0x");
                    outputBuilder.Write(bitwidthHexStringBacking);

                    if (bitfieldOffset != 0)
                    {
                        outputBuilder.Write(' ');
                        outputBuilder.Write("<<");
                        outputBuilder.Write(' ');
                        outputBuilder.Write(bitfieldOffset);
                        outputBuilder.Write(')');
                    }

                    outputBuilder.Write(')');
                    outputBuilder.Write(' ');
                    outputBuilder.Write('|');
                    outputBuilder.Write(' ');

                    if (canonicalTypeBacking != canonicalType)
                    {
                        outputBuilder.Write('(');
                        outputBuilder.Write(typeNameBacking);
                        outputBuilder.Write(')');
                    }

                    outputBuilder.Write('(');

                    if (bitfieldOffset != 0)
                    {
                        outputBuilder.Write('(');
                    }

                    outputBuilder.Write("value");
                    outputBuilder.Write(' ');
                    outputBuilder.Write('&');
                    outputBuilder.Write(' ');
                    outputBuilder.Write("0x");
                    outputBuilder.Write(bitwidthHexString);

                    if (bitfieldOffset != 0)
                    {
                        outputBuilder.Write(')');
                        outputBuilder.Write(' ');
                        outputBuilder.Write("<<");
                        outputBuilder.Write(' ');
                        outputBuilder.Write(bitfieldOffset);
                    }

                    outputBuilder.Write(')');

                    if (currentSize < 4)
                    {
                        outputBuilder.Write(')');
                    }

                    outputBuilder.WriteSemicolon();
                    outputBuilder.WriteNewline();
                    outputBuilder.WriteBlockEnd();
                    outputBuilder.WriteBlockEnd();
                }

                remainingBits -= fieldDecl.BitWidthValue;
                previousSize = Math.Max(previousSize, currentSize);
            }

            static void VisitConstantArrayFieldDecl(PInvokeGenerator pinvokeGenerator, RecordDecl recordDecl, FieldDecl constantArray)
            {
                Debug.Assert(constantArray.Type is ConstantArrayType);

                var outputBuilder = pinvokeGenerator._outputBuilder;
                var type = (ConstantArrayType)constantArray.Type;
                var typeName = pinvokeGenerator.GetRemappedTypeName(constantArray, context: null, constantArray.Type, out _);

                if (pinvokeGenerator.IsSupportedFixedSizedBufferType(typeName))
                {
                    return;
                }

                if (typeName.Contains('*'))
                {
                    outputBuilder.AddUsingDirective("System");
                    typeName = "IntPtr";
                }

                outputBuilder.NeedsNewline = true;

                var alignment = recordDecl.TypeForDecl.Handle.AlignOf;
                var maxAlignm = recordDecl.Fields.Max((fieldDecl) => fieldDecl.Type.Handle.AlignOf);

                if (alignment < maxAlignm)
                {
                    outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    outputBuilder.WriteIndented("[StructLayout(LayoutKind.Sequential");

                    outputBuilder.Write(',');
                    outputBuilder.Write(' ');
                    outputBuilder.Write("Pack");
                    outputBuilder.Write(' ');
                    outputBuilder.Write('=');
                    outputBuilder.Write(' ');
                    outputBuilder.Write(alignment);
                    outputBuilder.Write(')');
                    outputBuilder.WriteLine(']');
                }

                outputBuilder.WriteIndented(pinvokeGenerator.GetAccessSpecifierName(constantArray));
                outputBuilder.Write(' ');
                outputBuilder.Write("partial struct");
                outputBuilder.Write(' ');
                outputBuilder.WriteLine(pinvokeGenerator.GetArtificalFixedSizedBufferName(constantArray));
                outputBuilder.WriteBlockStart();

                var totalSize = type.Size;
                var sizePerDimension = new List<(long index, long size)>() {
                    (0, type.Size)
                };

                var elementType = type.ElementType;

                while (elementType is ConstantArrayType subConstantArrayType)
                {
                    totalSize *= subConstantArrayType.Size;
                    sizePerDimension.Add((0, subConstantArrayType.Size));
                    elementType = subConstantArrayType.ElementType;
                }

                for (long i = 0; i < totalSize; i++)
                {
                    outputBuilder.WriteIndented("internal");
                    outputBuilder.Write(' ');
                    outputBuilder.Write(typeName);
                    outputBuilder.Write(' ');
                    outputBuilder.Write('e');

                    var dimension = sizePerDimension[0];
                    outputBuilder.Write(dimension.index++);
                    sizePerDimension[0] = dimension;

                    for (int d = 1; d < sizePerDimension.Count; d++)
                    {
                        dimension = sizePerDimension[d];
                        outputBuilder.Write('_');
                        outputBuilder.Write(dimension.index);
                        sizePerDimension[d] = dimension;

                        var previousDimension = sizePerDimension[d - 1];

                        if (previousDimension.index == previousDimension.size)
                        {
                            previousDimension.index = 0;
                            dimension.index++;
                            sizePerDimension[d - 1] = previousDimension;
                            outputBuilder.NeedsNewline = true;
                        }

                        sizePerDimension[d] = dimension;
                    }

                    if (outputBuilder.NeedsNewline)
                    {
                        outputBuilder.WriteSemicolon();
                        outputBuilder.WriteNewline();
                        outputBuilder.NeedsNewline = true;
                    }
                    else
                    {
                        outputBuilder.WriteSemicolon();
                        outputBuilder.WriteNewline();
                    }
                }

                outputBuilder.NeedsNewline = true;
                outputBuilder.WriteIndented("public");
                outputBuilder.Write(' ');

                if (pinvokeGenerator._config.GenerateCompatibleCode)
                {
                    outputBuilder.Write("unsafe");
                    outputBuilder.Write(' ');
                }
                else
                {
                    outputBuilder.AddUsingDirective("System");
                    outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                }

                outputBuilder.Write("ref");
                outputBuilder.Write(' ');
                outputBuilder.Write(typeName);
                outputBuilder.Write(' ');

                if (pinvokeGenerator._config.GenerateCompatibleCode)
                {
                    outputBuilder.WriteLine("this[int index]");
                    outputBuilder.WriteBlockStart();
                    outputBuilder.WriteIndentedLine("get");
                    outputBuilder.WriteBlockStart();
                    outputBuilder.WriteIndented("fixed");
                    outputBuilder.Write(' ');
                    outputBuilder.Write('(');
                    outputBuilder.Write(typeName);
                    outputBuilder.Write('*');
                    outputBuilder.Write(' ');
                    outputBuilder.Write("pThis");
                    outputBuilder.Write(' ');
                    outputBuilder.Write('=');
                    outputBuilder.Write(' ');
                    outputBuilder.Write('&');
                    outputBuilder.Write("e0");
                    outputBuilder.WriteLine(')');
                    outputBuilder.WriteBlockStart();
                    outputBuilder.WriteIndented("return");
                    outputBuilder.Write(' ');
                    outputBuilder.Write("ref");
                    outputBuilder.Write(' ');
                    outputBuilder.Write("pThis");
                    outputBuilder.Write('[');
                    outputBuilder.Write("index");
                    outputBuilder.Write(']');
                    outputBuilder.WriteSemicolon();
                    outputBuilder.WriteNewline();
                    outputBuilder.WriteBlockEnd();
                    outputBuilder.WriteBlockEnd();
                    outputBuilder.WriteBlockEnd();
                }
                else
                {
                    outputBuilder.Write("this[int index] => ref AsSpan(");

                    if (type.Size == 1)
                    {
                        outputBuilder.Write("int.MaxValue");
                    }

                    outputBuilder.Write(')');
                    outputBuilder.Write('[');
                    outputBuilder.Write("index");
                    outputBuilder.Write(']');
                    outputBuilder.WriteSemicolon();
                    outputBuilder.WriteNewline();
                    outputBuilder.NeedsNewline = true;
                    outputBuilder.WriteIndented("public");
                    outputBuilder.Write(' ');
                    outputBuilder.Write("Span<");
                    outputBuilder.Write(typeName);
                    outputBuilder.Write('>');
                    outputBuilder.Write(' ');
                    outputBuilder.Write("AsSpan(");

                    if (type.Size == 1)
                    {
                        outputBuilder.Write("int length");
                    }

                    outputBuilder.Write(')');
                    outputBuilder.Write(' ');
                    outputBuilder.Write("=> MemoryMarshal.CreateSpan(ref e0,");
                    outputBuilder.Write(' ');

                    if (type.Size == 1)
                    {
                        outputBuilder.Write("length");
                    }
                    else
                    {
                        outputBuilder.Write(totalSize);
                    }

                    outputBuilder.Write(')');
                    outputBuilder.WriteSemicolon();
                    outputBuilder.WriteNewline();
                }

                outputBuilder.WriteBlockEnd();
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

                StartUsingOutputBuilder(name);
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                    _outputBuilder.WriteIndented("[UnmanagedFunctionPointer");

                    var callingConventionName = GetCallingConventionName(typedefDecl, (parentType is AttributedType) ? parentType.Handle.FunctionTypeCallingConv : functionProtoType.CallConv, name);

                    _outputBuilder.Write('(');
                    _outputBuilder.Write("CallingConvention");
                    _outputBuilder.Write('.');
                    _outputBuilder.Write(callingConventionName);
                    _outputBuilder.Write(')');

                    _outputBuilder.WriteLine(']');

                    var returnType = functionProtoType.ReturnType;
                    var returnTypeName = GetRemappedTypeName(typedefDecl, context: null, returnType, out var nativeTypeName);
                    AddNativeTypeNameAttribute(nativeTypeName, attributePrefix: "return: ");

                    _outputBuilder.WriteIndented(GetAccessSpecifierName(typedefDecl));
                    _outputBuilder.Write(' ');

                    if (IsUnsafe(typedefDecl, functionProtoType))
                    {
                        _outputBuilder.Write("unsafe");
                        _outputBuilder.Write(' ');
                    }

                    _outputBuilder.Write("delegate");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(returnTypeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(EscapeName(name));
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
                if (underlyingType is AttributedType attributedType)
                {
                    ForUnderlyingType(typedefDecl, attributedType.ModifiedType);
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
                else if (underlyingType is TypedefType typedefType)
                {
                    ForUnderlyingType(typedefDecl, typedefType.Decl.UnderlyingType);
                }
                else if (!(underlyingType is ArrayType) && !(underlyingType is BuiltinType) && !(underlyingType is TagType))
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported underlying type: '{underlyingType.TypeClass}'. Generating bindings may be incomplete.", typedefDecl);
                }
                return;
            }
        }

        private void VisitVarDecl(VarDecl varDecl)
        {
            var previousContext = PreviousContext;

            if (previousContext is DeclStmt declStmt)
            {
                ForDeclStmt(varDecl, declStmt);
            }
            else if ((previousContext is TranslationUnitDecl) || (previousContext is LinkageSpecDecl) || (previousContext is RecordDecl))
            {
                if (!varDecl.HasInit)
                {
                    // Nothing to do if a top level const declaration doesn't have an initializer
                    return;
                }

                var isMacroDefinitionRecord = false;

                var nativeName = GetCursorName(varDecl);
                if (nativeName.StartsWith("ClangSharpMacro_"))
                {
                    nativeName = nativeName.Substring("ClangSharpMacro_".Length);
                    isMacroDefinitionRecord = true;
                }
                var name = GetRemappedName(nativeName, varDecl, tryRemapOperatorName: false);

                var type = varDecl.Type;
                var openedOutputBuilder = false;

                if (_outputBuilder is null)
                {
                    StartUsingOutputBuilder(_config.MethodClassName);
                    openedOutputBuilder = true;
                }

                WithAttributes("*");
                WithAttributes(name);

                WithUsings("*");
                WithUsings(name);

                var typeName = GetRemappedTypeName(varDecl, context: null, type, out var nativeTypeName);

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

                _outputBuilder.WriteIndented(GetAccessSpecifierName(varDecl));
                _outputBuilder.Write(' ');

                var initExpr = varDecl.Init;

                if (initExpr is ImplicitCastExpr implicitCastExpr)
                {
                    initExpr = implicitCastExpr.SubExprAsWritten;
                }

                var isProperty = false;

                if (initExpr is StringLiteral stringLiteral)
                {
                    switch (stringLiteral.Kind)
                    {
                        case CX_CharacterKind.CX_CLK_Ascii:
                        case CX_CharacterKind.CX_CLK_UTF8:
                        {
                            _outputBuilder.Write("static");
                            _outputBuilder.Write(' ');
                            _outputBuilder.AddUsingDirective("System");

                            typeName = "ReadOnlySpan<byte>";
                            isProperty = true;
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
                            _outputBuilder.Write("const");
                            _outputBuilder.Write(' ');

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
                else if (type.IsLocalConstQualified && CanBeConstant(type, varDecl.Init))
                {
                    _outputBuilder.Write("const");
                    _outputBuilder.Write(' ');
                }
                else if ((varDecl.StorageClass == CX_StorageClass.CX_SC_Static) || openedOutputBuilder)
                {
                    _outputBuilder.Write("static");
                    _outputBuilder.Write(' ');

                    if (type.IsLocalConstQualified)
                    {
                        _outputBuilder.Write("readonly");
                        _outputBuilder.Write(' ');
                    }
                }

                _outputBuilder.Write(typeName);

                if (type is ArrayType)
                {
                    _outputBuilder.Write('[');
                    _outputBuilder.Write(']');
                }

                _outputBuilder.Write(' ');

                _outputBuilder.Write(EscapeName(name));

                if (varDecl.HasInit)
                {
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write('=');

                    if (isProperty)
                    {
                        _outputBuilder.Write('>');
                    }

                    _outputBuilder.Write(' ');

                    Visit(varDecl.Init);
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
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported variable declaration parent: '{previousContext.CursorKindSpelling}'. Generated bindings may be incomplete.", previousContext);
            }

            void ForDeclStmt(VarDecl varDecl, DeclStmt declStmt)
            {
                var name = GetRemappedCursorName(varDecl);

                if (varDecl == declStmt.Decls.First())
                {
                    var type = varDecl.Type;
                    var typeName = GetRemappedTypeName(varDecl, context: null, type, out var nativeTypeName);

                    _outputBuilder.Write(typeName);

                    if (type is ArrayType)
                    {
                        _outputBuilder.Write('[');
                        _outputBuilder.Write(']');
                    }

                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write(EscapeName(name));

                if (varDecl.HasInit)
                {
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write('=');
                    _outputBuilder.Write(' ');

                    Visit(varDecl.Init);
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
                // case CX_StmtClass.CX_StmtClass_ConditionalOperator:
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
                // case CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr:
                // case CX_StmtClass.CX_StmtClass_UserDefinedLiteral:
                // case CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr:

                case CX_StmtClass.CX_StmtClass_CStyleCastExpr:
                {
                    var cStyleCastExpr = (CStyleCastExpr)initExpr;
                    return IsConstant(cStyleCastExpr.SubExpr);
                }

                // case CX_StmtClass.CX_StmtClass_CXXFunctionalCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXConstCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXStaticCastExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr:

                case CX_StmtClass.CX_StmtClass_ImplicitCastExpr:
                {
                    var implicitCastExpr = (ImplicitCastExpr)initExpr;
                    return IsConstant(implicitCastExpr.SubExpr);
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
                           ((declRefExpr.Decl is VarDecl varDecl) && IsConstant(varDecl.Init));
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
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported VarDecl.InitExpr: '{initExpr.StmtClassName}'. Generated bindings may not be constant.", initExpr);
                    return false;
                }
            }
        }
    }
}
