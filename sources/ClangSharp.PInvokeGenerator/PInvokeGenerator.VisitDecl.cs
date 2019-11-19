// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public partial class PInvokeGenerator
    {
        private void VisitDecl(Decl decl)
        {
            if (decl is NamedDecl namedDecl)
            {
                // We get the non-remapped name for the purpose of exclusion
                // checks to ensure that users can remove no-definition declarations
                // in favor of remapped anonymous declarations.
                var name = GetCursorName(namedDecl);

                if (_config.ExcludedNames.Contains(name))
                {
                    return;
                }

                if (decl is TagDecl tagDecl)
                {
                    if ((tagDecl.Definition != tagDecl) && (tagDecl.Definition != null))
                    {
                        // We don't want to generate bindings for anything
                        // that is not itself a definition and that has a
                        // definition that can be resolved. This ensures we
                        // still generate bindings for things which are used
                        // as opaque handles, but which aren't ever defined.

                        return;
                    }
                }
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
                // case CX_DeclKind.CX_DeclKind_Empty:
                // case CX_DeclKind.CX_DeclKind_Export:
                // case CX_DeclKind.CX_DeclKind_ExternCContext:
                // case CX_DeclKind.CX_DeclKind_FileScopeAsm:
                // case CX_DeclKind.CX_DeclKind_Friend:
                // case CX_DeclKind.CX_DeclKind_FriendTemplate:
                // case CX_DeclKind.CX_DeclKind_Import:

                case CX_DeclKind.CX_DeclKind_LinkageSpec:
                {
                    // Linkage specifications are also exposed as a queryable property
                    // on the declarations they impact, so we don't need to do anything
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_Label:
                // case CX_DeclKind.CX_DeclKind_Namespace:
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
                // case CX_DeclKind.CX_DeclKind_ClassTemplate:
                // case CX_DeclKind.CX_DeclKind_FunctionTemplate:
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
                    VisitRecordDecl((CXXRecordDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_ClassTemplateSpecialization:
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
                // case CX_DeclKind.CX_DeclKind_Using:
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
                {
                    VisitFunctionDecl((FunctionDecl)decl);
                    break;
                }

                // case CX_DeclKind.CX_DeclKind_CXXDeductionGuide:
                // case CX_DeclKind.CX_DeclKind_CXXMethod:
                // case CX_DeclKind.CX_DeclKind_CXXConstructor:
                // case CX_DeclKind.CX_DeclKind_CXXConversion:
                // case CX_DeclKind.CX_DeclKind_CXXDestructor:
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
                // case CX_DeclKind.CX_DeclKind_PragmaComment:
                // case CX_DeclKind.CX_DeclKind_PragmaDetectMismatch:
                // case CX_DeclKind.CX_DeclKind_StaticAssert:
                // case CX_DeclKind.CX_DeclKind_TranslationUnit:

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported declaration: '{decl.Kind}'. Generated bindings may be incomplete.", decl);
                    break;
                }
            }

            if (decl is IDeclContext declContext)
            {
                VisitDecls(declContext.Decls);
            }
        }

        private void VisitDecls(IEnumerable<Decl> decls)
        {
            foreach (var decl in decls)
            {
                Visit(decl);
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
                var integerTypeName = GetRemappedTypeName(enumDecl, enumDecl.IntegerType, out var nativeTypeName);
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
        
                _outputBuilder.WriteLine();
                _outputBuilder.WriteBlockStart();

                VisitDecls(enumDecl.Enumerators);
                VisitDecls(enumDecl.Decls);
        
                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitFieldDecl(FieldDecl fieldDecl)
        {
            var name = GetRemappedCursorName(fieldDecl);
            var escapedName = EscapeName(name);

            var type = fieldDecl.Type;
            var typeName = GetRemappedTypeName(fieldDecl, type, out var nativeTypeName);

            if (fieldDecl.Parent.IsUnion)
            {
                _outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
            }
            AddNativeTypeNameAttribute(nativeTypeName);

            _outputBuilder.WriteIndented(GetAccessSpecifierName(fieldDecl));
            _outputBuilder.Write(' ');

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
                    _outputBuilder.Write(constantArrayType.Size);
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
                if (fieldDecl.IsBitField)
                {
                    AddDiagnostic(DiagnosticLevel.Warning, "Unsupported field declaration kind: 'BitField'. Generated bindings may be incomplete.", fieldDecl);
                }

                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(escapedName);
            }

            _outputBuilder.WriteLine(';');
        }

        private void VisitFunctionDecl(FunctionDecl functionDecl)
        {
            var name = GetRemappedCursorName(functionDecl);

            StartUsingOutputBuilder(_config.MethodClassName);
            {
                var type = functionDecl.Type;

                if (type is AttributedType attributedType)
                {
                    type = attributedType.ModifiedType;
                }
                var functionType = (FunctionType)type;

                var body = functionDecl.Body;

                if (body is null)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                    _outputBuilder.WriteIndented("[DllImport(LibraryPath, CallingConvention = CallingConvention.");
                    _outputBuilder.Write(GetCallingConventionName(functionDecl, functionType.CallConv));
                    _outputBuilder.Write(", EntryPoint = \"");
                    _outputBuilder.Write(name);
                    _outputBuilder.WriteLine("\", ExactSpelling = true)]");
                }

                var returnType = functionDecl.ReturnType;
                var returnTypeName = GetRemappedTypeName(functionDecl, returnType, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName, attributePrefix: "return: ");

                _outputBuilder.WriteIndented(GetAccessSpecifierName(functionDecl));
                _outputBuilder.Write(' ');
                _outputBuilder.Write("static");
                _outputBuilder.Write(' ');

                if (body is null)
                {
                    _outputBuilder.Write("extern");
                    _outputBuilder.Write(' ');
                }

                if (IsUnsafe(functionDecl))
                {
                    _isMethodClassUnsafe = true;
                }

                _outputBuilder.Write(returnTypeName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeAndStripName(name));
                _outputBuilder.Write('(');

                VisitDecls(functionDecl.Parameters);

                _outputBuilder.Write(")");

                if (body is null)
                {
                    _outputBuilder.WriteLine(';');
                }
                else
                {
                    _outputBuilder.WriteLine();

                    if (body is CompoundStmt)
                    {
                        Visit(body);
                    }
                    else
                    {
                        _outputBuilder.WriteBlockStart();
                        Visit(body);
                        _outputBuilder.WriteBlockEnd();
                    }
                }

                VisitDecls(functionDecl.Decls);
            }
            StopUsingOutputBuilder();
        }

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl)
        {
            var cursorParent = parmVarDecl.CursorParent;
        
            if (cursorParent is FunctionDecl functionDecl)
            {
                VisitParmVarDeclForFunctionDecl(parmVarDecl, functionDecl);
            }
            else if (cursorParent is TypedefDecl typedefDecl)
            {
                VisitParmVarDeclForTypedefDecl(parmVarDecl, typedefDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported parameter variable declaration parent: '{cursorParent.CursorKindSpelling}'. Generated bindings may be incomplete.", cursorParent);
            }
        }
        
        private void VisitParmVarDeclForFunctionDecl(ParmVarDecl parmVarDecl, FunctionDecl functionDecl)
        {
            var type = parmVarDecl.Type;
            var typeName = GetRemappedTypeName(parmVarDecl, type, out var nativeTypeName);
            AddNativeTypeNameAttribute(nativeTypeName, prefix: "", postfix: " ");
        
            _outputBuilder.Write(typeName);
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
        
            if (index != lastIndex)
            {
                _outputBuilder.Write(',');
                _outputBuilder.Write(' ');
            }
        }
        
        private void VisitParmVarDeclForTypedefDecl(ParmVarDecl parmVarDecl, TypedefDecl typedefDecl)
        {
            var type = parmVarDecl.Type;
            var typeName = GetRemappedTypeName(parmVarDecl, type, out var nativeTypeName);
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
        
            if (index != lastIndex)
            {
                _outputBuilder.Write(',');
                _outputBuilder.Write(' ');
            }
        }

        private void VisitRecordDecl(RecordDecl recordDecl)
        {
            var name = GetRemappedCursorName(recordDecl);

            StartUsingOutputBuilder(name);
            {
                if (recordDecl.IsUnion)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    _outputBuilder.WriteIndentedLine("[StructLayout(LayoutKind.Explicit)]");
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

                var fields = recordDecl.Fields;

                if (fields.Count != 0)
                {
                    Visit(fields[0]);

                    for (int i = 1; i < fields.Count; i++)
                    {
                        _outputBuilder.WriteLine();
                        Visit(fields[i]);
                    }
                }

                VisitDecls(recordDecl.Decls);

                foreach (var constantArray in recordDecl.Fields.Where((field) => field.Type is ConstantArrayType))
                {
                    var type = (ConstantArrayType)constantArray.Type;
                    var typeName = GetRemappedTypeName(constantArray, constantArray.Type, out _);

                    if (IsSupportedFixedSizedBufferType(typeName))
                    {
                        continue;
                    }
                    bool isUnsafe = typeName.Contains('*');

                    _outputBuilder.WriteLine();
                    _outputBuilder.WriteIndented(GetAccessSpecifierName(constantArray));
                    _outputBuilder.Write(' ');

                    if (isUnsafe)
                    {
                        _outputBuilder.Write("unsafe");
                        _outputBuilder.Write(' ');
                    }

                    _outputBuilder.Write("partial struct");
                    _outputBuilder.Write(' ');
                    _outputBuilder.WriteLine(GetArtificalFixedSizedBufferName(constantArray));
                    _outputBuilder.WriteBlockStart();

                    for (int i = 0; i < type.Size; i++)
                    {
                        _outputBuilder.WriteIndented("internal");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(typeName);
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write('e');
                        _outputBuilder.Write(i);
                        _outputBuilder.WriteLine(';');
                    }

                    _outputBuilder.WriteLine();
                    _outputBuilder.WriteIndented("public");
                    _outputBuilder.Write(' ');

                    if (!isUnsafe)
                    {
                        _outputBuilder.Write("unsafe");
                        _outputBuilder.Write(' ');
                    }

                    _outputBuilder.Write("ref");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.WriteLine("this[int index]");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndentedLine("get");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndented("fixed (");
                    _outputBuilder.Write(typeName);
                    _outputBuilder.WriteLine("* pThis = &e0)");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndentedLine("return ref pThis[index];");
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();
                }

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitTypedefDecl(TypedefDecl typedefDecl)
        {
            VisitTypedefDeclForUnderlyingType(typedefDecl, typedefDecl.UnderlyingType);
        }

        private void VisitTypedefDeclForUnderlyingType(TypedefDecl typedefDecl, Type underlyingType)
        {
            if (underlyingType is ElaboratedType elaboratedType)
            {
                VisitTypedefDeclForUnderlyingType(typedefDecl, elaboratedType.NamedType);
            }
            else if (underlyingType is PointerType pointerType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, pointerType.PointeeType);
            }
            else if (underlyingType is TypedefType typedefType)
            {
                VisitTypedefDeclForUnderlyingType(typedefDecl, typedefType.Decl.UnderlyingType);
            }
            else if (!(underlyingType is BuiltinType) && !(underlyingType is IncompleteArrayType) && !(underlyingType is TagType))
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported underlying type: '{underlyingType.TypeClass}'. Generating bindings may be incomplete.", typedefDecl);
            }
            return;
        }
        
        private void VisitTypedefDeclForPointeeType(TypedefDecl typedefDecl, Type pointeeType)
        {
            if (pointeeType is AttributedType attributedType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, attributedType.ModifiedType);
            }
            else if (pointeeType is ElaboratedType elaboratedType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, elaboratedType.NamedType);
            }
            else if (pointeeType is FunctionProtoType functionProtoType)
            {
                var name = GetRemappedCursorName(typedefDecl);
        
                StartUsingOutputBuilder(name);
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
        
                    _outputBuilder.WriteIndented("[UnmanagedFunctionPointer(CallingConvention.");
                    _outputBuilder.Write(GetCallingConventionName(typedefDecl, functionProtoType.CallConv));
                    _outputBuilder.WriteLine(")]");
        
                    var returnType = functionProtoType.ReturnType;
                    var returnTypeName = GetRemappedTypeName(typedefDecl, returnType, out var nativeTypeName);
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

                    VisitDecls(typedefDecl.CursorChildren.OfType<ParmVarDecl>());
        
                    _outputBuilder.WriteLine(");");
                }
                StopUsingOutputBuilder();
            }
            else if (pointeeType is PointerType pointerType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, pointerType.PointeeType);
            }
            else if (pointeeType is TypedefType typedefType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, typedefType.Decl.UnderlyingType);
            }
            else if (!(pointeeType is BuiltinType) && !(pointeeType is TagType))
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported pointee type: '{pointeeType.TypeClass}'. Generating bindings may be incomplete.", typedefDecl);
            }
        }

        private void VisitVarDecl(VarDecl varDecl)
        {
            var name = GetRemappedCursorName(varDecl);

            StartUsingOutputBuilder(_config.MethodClassName);
            {
                var type = varDecl.Type;
                var typeName = GetRemappedTypeName(varDecl, type, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName, prefix: "// ");

                _outputBuilder.WriteIndented("// public static extern");
                _outputBuilder.Write(' ');
                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeName(name));
                _outputBuilder.WriteLine(';');
            }
            StopUsingOutputBuilder();
        }
    }
}
