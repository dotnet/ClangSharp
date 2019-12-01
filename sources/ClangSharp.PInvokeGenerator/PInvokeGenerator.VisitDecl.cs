// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                case CX_DeclKind.CX_DeclKind_StaticAssert:
                {
                    // Static asserts can't be easily modeled in C#
                    // We'll ignore them for now.
                    break;
                }

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
            if (fieldDecl.IsBitField)
            {
                return;
            }

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

                var fieldCount = 0;
                var bitfieldCount = GetBitfieldCount(this, recordDecl);

                var bitfieldIndex = (bitfieldCount == 1) ? -1 : 0;
                var bitfieldPreviousSize = 0L;
                var bitfieldRemainingBits = 0L;

                foreach (var declaration in recordDecl.Decls)
                {
                    if (declaration is FieldDecl fieldDecl)
                    {
                        if (fieldCount != 0)
                        {
                            _outputBuilder.WriteLine();
                        }
                        fieldCount++;

                        if (fieldDecl.IsBitField)
                        {
                            VisitBitfieldDecl(this, fieldDecl, ref bitfieldIndex, ref bitfieldPreviousSize, ref bitfieldRemainingBits);
                        }
                        Visit(fieldDecl);
                    }
                    else if ((declaration is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion)
                    {
                        if (fieldCount != 0)
                        {
                            _outputBuilder.WriteLine();
                        }
                        fieldCount++;

                        var nestedRecordDeclName = GetRemappedTypeName(nestedRecordDecl, nestedRecordDecl.TypeForDecl, out string nativeTypeName);

                        if (recordDecl.IsUnion)
                        {
                            _outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
                        }
                        AddNativeTypeNameAttribute(nativeTypeName);

                        _outputBuilder.WriteIndented(GetAccessSpecifierName(nestedRecordDecl));
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(nestedRecordDeclName);
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(GetRemappedAnonymousName(nestedRecordDecl, "Field"));
                        _outputBuilder.WriteLine(';');
                    }
                }

                VisitDecls(recordDecl.Decls);

                foreach (var constantArray in recordDecl.Fields.Where((field) => field.Type is ConstantArrayType))
                {
                    VisitConstantArrayFieldDecl(this, constantArray);
                }

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();

            static int GetBitfieldCount(PInvokeGenerator pinvokeGenerator, RecordDecl recordDecl)
            {
                var count = 0;
                var previousSize = 0L;
                var remainingBits = 0L;

                foreach (var fieldDecl in recordDecl.Fields)
                {
                    if (!fieldDecl.IsBitField)
                    {
                        continue;
                    }

                    var currentSize = fieldDecl.Type.Handle.SizeOf;

                    if ((currentSize != previousSize) || (fieldDecl.BitWidthValue > remainingBits))
                    {
                        count++;
                        remainingBits = currentSize * 8;
                    }

                    remainingBits -= fieldDecl.BitWidthValue;
                    previousSize = currentSize;
                }

                return count;
            }

            static void VisitBitfieldDecl(PInvokeGenerator pinvokeGenerator, FieldDecl fieldDecl, ref int index, ref long previousSize, ref long remainingBits)
            {
                Debug.Assert(fieldDecl.IsBitField);

                var outputBuilder = pinvokeGenerator._outputBuilder;
                var typeName = pinvokeGenerator.GetRemappedTypeName(fieldDecl, fieldDecl.Type, out var nativeTypeName);

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

                if ((currentSize != previousSize) || (fieldDecl.BitWidthValue > remainingBits))
                {
                    index++;

                    if (index > 0)
                    {
                        bitfieldName += index.ToString();
                    }
                    remainingBits = currentSize * 8;

                    outputBuilder.WriteIndented("internal");
                    outputBuilder.Write(' ');
                    outputBuilder.Write(typeName);
                    outputBuilder.Write(' ');
                    outputBuilder.Write(bitfieldName);
                    outputBuilder.WriteLine(';');
                    outputBuilder.WriteLine();
                }
                else if (index > 0)
                {
                    bitfieldName += index.ToString();
                }
                pinvokeGenerator.AddNativeTypeNameAttribute(nativeTypeName);

                var bitfieldOffset = (currentSize * 8) - remainingBits;
                var bitwidthHexString = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X");
                var canonicalType = fieldDecl.Type.CanonicalType;

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

                if (currentSize < 4)
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
                outputBuilder.Write(bitwidthHexString);

                if (currentSize < 4)
                {
                    outputBuilder.Write(')');
                }

                outputBuilder.WriteLine(';');
                outputBuilder.WriteBlockEnd();
                outputBuilder.WriteLine();

                outputBuilder.WriteIndentedLine("set");
                outputBuilder.WriteBlockStart();
                outputBuilder.WriteIndented(bitfieldName);
                outputBuilder.Write(' ');
                outputBuilder.Write('=');
                outputBuilder.Write(' ');

                if (currentSize < 4)
                {
                    outputBuilder.Write('(');
                    outputBuilder.Write(typeName);
                    outputBuilder.Write(')');
                    outputBuilder.Write('(');
                }

                outputBuilder.Write('(');
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
                outputBuilder.Write(bitwidthHexString);

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

                outputBuilder.WriteLine(';');
                outputBuilder.WriteBlockEnd();
                outputBuilder.WriteBlockEnd();

                remainingBits -= fieldDecl.BitWidthValue;
                previousSize = currentSize;
            }

            static void VisitConstantArrayFieldDecl(PInvokeGenerator pinvokeGenerator, FieldDecl constantArray)
            {
                Debug.Assert(constantArray.Type is ConstantArrayType);

                var outputBuilder = pinvokeGenerator._outputBuilder;
                var type = (ConstantArrayType)constantArray.Type;
                var typeName = pinvokeGenerator.GetRemappedTypeName(constantArray, constantArray.Type, out _);

                if (pinvokeGenerator.IsSupportedFixedSizedBufferType(typeName))
                {
                    return;
                }
                bool isUnsafe = typeName.Contains('*');

                outputBuilder.WriteLine();
                outputBuilder.WriteIndented(pinvokeGenerator.GetAccessSpecifierName(constantArray));
                outputBuilder.Write(' ');

                if (isUnsafe)
                {
                    outputBuilder.Write("unsafe");
                    outputBuilder.Write(' ');
                }

                outputBuilder.Write("partial struct");
                outputBuilder.Write(' ');
                outputBuilder.WriteLine(pinvokeGenerator.GetArtificalFixedSizedBufferName(constantArray));
                outputBuilder.WriteBlockStart();

                for (int i = 0; i < type.Size; i++)
                {
                    outputBuilder.WriteIndented("internal");
                    outputBuilder.Write(' ');
                    outputBuilder.Write(typeName);
                    outputBuilder.Write(' ');
                    outputBuilder.Write('e');
                    outputBuilder.Write(i);
                    outputBuilder.WriteLine(';');
                }

                outputBuilder.WriteLine();
                outputBuilder.WriteIndented("public");
                outputBuilder.Write(' ');

                if (pinvokeGenerator._config.GenerateCompatibleCode)
                {
                    if (!isUnsafe)
                    {
                        outputBuilder.Write("unsafe");
                        outputBuilder.Write(' ');
                    }
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
                    outputBuilder.WriteIndented("fixed (");
                    outputBuilder.Write(typeName);
                    outputBuilder.WriteLine("* pThis = &e0)");
                    outputBuilder.WriteBlockStart();
                    outputBuilder.WriteIndentedLine("return ref pThis[index];");
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

                    outputBuilder.WriteLine(")[index];");
                    outputBuilder.WriteLine();
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
                        outputBuilder.Write(type.Size);
                    }

                    outputBuilder.Write(')');
                    outputBuilder.WriteLine(';');
                }

                outputBuilder.WriteBlockEnd();
            }
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
