using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using ClangSharp.Abstractions;

namespace ClangSharp.CSharp
{
    public partial class CSharpOutputBuilder : IOutputBuilder
    {
        public void BeginInnerValue() => Write('(');
        public void EndInnerValue() => Write(')');

        public void BeginInnerCast() => Write('(');
        public void WriteCastType(string targetTypeName) => Write(targetTypeName);
        public void EndInnerCast() => Write('(');

        public void BeginUnchecked() => Write("unchecked");
        public void EndUnchecked()
        {
            // nop, used only by XML
        }

        public void BeginConstant(string accessSpecifier, string typeName, string escapedName, ConstantKind kind)
        {
            WriteIndentation();

            if ((kind & ConstantKind.PrimitiveConstant) != 0)
            {
                Write(accessSpecifier);
                Write(" const ");
                Write(typeName);
                Write(' ');
            }
            else if ((kind & ConstantKind.NonPrimitiveConstant) != 0)
            {
                Write(accessSpecifier);
                Write(" static ");
                if ((kind & ConstantKind.ReadOnly) != 0)
                {
                    Write(" readonly ");
                }

                Write(typeName);
                Write(' ');
            }

            Write(escapedName);
        }

        public void BeginConstantValue(bool isGetOnlyProperty = false)
        {
            Write(isGetOnlyProperty ? " => " : " = ");
        }

        public void WriteConstantValue(long value) => Write(value);
        public void WriteConstantValue(ulong value) => Write(value);

        public void EndConstantValue()
        {
            // nop, used only by the XML backend
        }

        public void EndConstant(bool isConstant) => WriteLine(isConstant ? ';' : ',');

        public void BeginEnum(string accessSpecifier, string typeName, string escapedName, string nativeTypeName)
        {
            if (nativeTypeName is not null)
            {
                AddNativeTypeNameAttribute(nativeTypeName);
            }

            WriteIndented(accessSpecifier);
            Write(" enum ");
            Write(escapedName);

            if (!typeName.Equals("int"))
            {
                Write(" : ");
                Write(typeName);
            }

            NeedsNewline = true;
            WriteBlockStart();
        }

        public void EndEnum() => WriteBlockEnd();

        public void BeginField(string accessSpecifier, string nativeTypeName, string escapedName, int? offset,
            bool needsNewKeyword, string inheritedFrom = null)
        {
            if (offset is not null)
            {
                WriteIndentedLine($"[FieldOffset({offset})]");
            }

            if (nativeTypeName is not null)
            {
                AddNativeTypeNameAttribute(nativeTypeName);
            }

            WriteIndented(accessSpecifier);
            Write(' ');

            if (needsNewKeyword)
            {
                Write("new ");
            }
        }

        public void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count)
        {
            if (PInvokeGenerator.IsSupportedFixedSizedBufferType(typeName))
            {
                Write("fixed ");
                Write(typeName);
                Write(' ');
                Write(escapedName);
                Write('[');
                Write(count);
                Write(']');
            }
            else
            {
                Write(fixedName);
                Write(' ');
                Write(escapedName);
            }
        }

        public void WriteRegularField(string typeName, string escapedName)
        {
            Write(typeName);
            Write(' ');
            Write(escapedName);
        }

        public void EndField()
        {
            WriteSemicolon();
            WriteNewline();
            NeedsNewline = true;
        }

        public void BeginFunctionOrDelegate<TCustomAttrGeneratorData>(
            in FunctionOrDelegateDesc<TCustomAttrGeneratorData> desc,
            ref bool isMethodClassUnsafe)
        {
            desc.WriteCustomAttrs(desc.CustomAttrGeneratorData);
            if (desc.IsVirtual)
            {
                Debug.Assert(!desc.HasFnPtrCodeGen);

                AddUsingDirective("System.Runtime.InteropServices");
                WriteIndented("[UnmanagedFunctionPointer");

                if (desc.CallingConventionName != "Winapi")
                {
                    Write("(CallingConvention.");
                    Write(desc.CallingConventionName);
                    Write(')');
                }

                WriteLine(']');
            }
            else if (desc.IsDllImport)
            {
                AddUsingDirective("System.Runtime.InteropServices");

                WriteIndented("[DllImport(");

                Write('"');
                Write(desc.LibraryPath);
                Write('"');

                Write(", ");

                if (desc.CallingConventionName != "Winapi")
                {
                    Write("CallingConvention = CallingConvention.");
                    Write(desc.CallingConventionName);
                    Write(", ");
                }

                if (desc.EntryPoint != desc.EscapedName)
                {
                    Write("EntryPoint = \"");
                    Write(desc.EntryPoint);
                    Write("\", ");
                }

                Write("ExactSpelling = true");
                if (desc.SetLastError)
                {
                    Write("SetLastError = true");
                }
                WriteLine(")]");
            }

            if (desc.IsAggressivelyInlined)
            {
                AddUsingDirective("System.Runtime.CompilerServices");
                WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            }

            if (desc.NativeTypeName is not null)
            {
                AddNativeTypeNameAttribute(desc.NativeTypeName, attributePrefix: "return: ");
            }

            WriteIndented(desc.AccessSpecifier);

            if (!desc.IsMemberFunction)
            {
                if (desc.IsVirtual)
                {
                    Write(" delegate");
                }
                else if (desc.IsDllImport || !desc.IsCxx || desc.IsStatic)
                {
                    Write(" static");

                    if (desc.IsDllImport)
                    {
                        Write(" extern");
                    }
                }
            }

            Write(' ');

            if (!desc.IsVirtual)
            {
                //if (NeedsNewKeyword(escapedName, functionDecl.Parameters))
                if (desc.NeedsNewKeyword)
                {
                    Write("new ");
                }

                if (desc.IsUnsafe)
                {
                    //if (cxxRecordDecl is null)
                    if (!desc.IsCtxCxxRecord)
                    {
                        isMethodClassUnsafe = true;
                    }
                    //else if (!IsUnsafe(cxxRecordDecl))
                    else if (!desc.IsCxxRecordCtxUnsafe)
                    {
                        Write("unsafe ");
                    }
                }
            }
        }

        public void WriteReturnType(string typeString)
        {
            Write(typeString);
            Write(' ');
        }

        public void BeginFunctionInnerPrototype(string escapedName)
        {
            Write(escapedName);
            Write('(');
        }

        public void BeginParameter<TCustomAttrGeneratorData>(in ParameterDesc<TCustomAttrGeneratorData> info)
        {
            if (info.NativeTypeName is not null)
            {
                AddNativeTypeNameAttribute(info.NativeTypeName, prefix: "", postfix: " ");
            }

            if (info.CppAttributes is not null)
            {
                AddCppAttributes(info.CppAttributes, prefix: "", postfix: " ");
            }

            _customAttrIsForParameter = true;
            info.WriteCustomAttrs(info.CustomAttrGeneratorData);
            _customAttrIsForParameter = false;
            Write(info.Type);
            Write(' ');
            Write(info.Name);
        }

        public void BeginParameterDefault()
        {
            Write(" = ");
        }

        public void EndParameterDefault()
        {
            // nop, used only by XML
        }

        public void EndParameter()
        {
            // nop, used only by XML
        }

        public void WriteParameterSeparator()
        {
            Write(',');
            Write(' ');
        }

        public void EndFunctionInnerPrototype()
        {
            Write(')');
        }

        public void BeginConstructorInitializer(string memberRefName, string memberInitName)
        {
            WriteIndentation();
            if (memberRefName.Equals(memberInitName))
            {
                Write("this");
                Write('.');
            }

            Write(memberRefName);
            Write(' ');
            Write('=');
            Write(' ');
        }

        public void EndConstructorInitializer()
        {
            WriteSemicolon();
            WriteNewline();
        }

        public void BeginBody(bool isExpressionBody = false)
        {
            if (isExpressionBody)
            {
                Write(" => ");
            }
            else
            {
                NeedsNewline = true;
                WriteBlockStart();
            }
        }

        public void BeginConstructorInitializers()
        {
            // nop, method only exists for consistency and/or future use
        }

        public void EndConstructorInitializers()
        {
            // nop, method only exists for consistency and/or future use
        }

        public void BeginInnerFunctionBody()
        {
            WriteIndentation();
        }

        public void EndInnerFunctionBody()
        {
            // nop, used only by XML
        }

        public void EndBody(bool isExpressionBody = false)
        {
            if (isExpressionBody)
            {
                return;
            }

            WriteSemicolonIfNeeded();
            WriteNewlineIfNeeded();
            WriteBlockEnd();
        }

        public void EndFunctionOrDelegate(bool isVirtual, bool isBodyless)
        {
            if (isBodyless)
            {
                WriteSemicolon();
                WriteNewline();
            }

            NeedsNewline = true;
        }

        public void BeginStruct<TCustomAttrGeneratorData>(in StructDesc<TCustomAttrGeneratorData> info)
        {
            if (info.Layout is not null)
            {
                AddUsingDirective("System.Runtime.InteropServices");
                WriteIndented("[StructLayout(LayoutKind.");
                Write(info.Layout.Value);
                if (info.Layout.Pack != default)
                {
                    Write(", Pack = ");
                    Write(info.Layout.Pack);
                }

                WriteLine(")]");
            }

            if (info.Uuid is not null)
            {
                AddUsingDirective("System.Runtime.InteropServices");

                WriteIndented("[Guid(\"");
                Write(info.Uuid.Value.ToString("D", CultureInfo.InvariantCulture).ToUpperInvariant());
                WriteLine("\")]");
            }

            if (info.NativeType is not null)
            {
                AddNativeTypeNameAttribute(info.NativeType);
            }

            if (info.NativeInheritance is not null)
            {
                AddNativeInheritanceAttribute(info.NativeInheritance);
            }

            WriteIndented(info.AccessSpecifier);
            Write(' ');

            if (info.IsUnsafe)
            {
                Write("unsafe ");
            }

            Write("partial struct ");
            Write(info.EscapedName);
            WriteNewline();
            WriteBlockStart();
        }

        public void BeginExplicitVtbl()
        {
            NeedsNewline = true;
            WriteIndentedLine("public partial struct Vtbl");
            WriteBlockStart();
        }

        public void EmitCompatibleCodeSupport()
        {
            AddUsingDirective("System.Runtime.CompilerServices");
        }

        public void EmitFnPtrSupport()
        {
            AddUsingDirective("System");
            AddUsingDirective("System.Runtime.InteropServices");
        }

        public void EmitSystemSupport()
        {
            AddUsingDirective("System");
        }

        public void EndStruct()
        {
            WriteBlockEnd();
        }

        public void EndExplicitVtbl()
        {
            WriteBlockEnd();
        }

        public CSharpOutputBuilder BeginCSharpCode()
        {
            // just write directly to this buffer
            return this;
        }

        public void EndCSharpCode(CSharpOutputBuilder output)
        {
            // nop, used only by XML
        }

        public void BeginGetter(bool aggressivelyInlined)
        {
            if (aggressivelyInlined)
            {
                AddUsingDirective("System.Runtime.CompilerServices");
                WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            }

            WriteIndentedLine("get");
            WriteBlockStart();
        }

        public void EndGetter()
        {
            WriteBlockEnd();
            WriteNewline();
        }

        public void BeginSetter(bool aggressivelyInlined)
        {
            if (aggressivelyInlined)
            {
                AddUsingDirective("System.Runtime.CompilerServices");
                WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            }

            WriteIndentedLine("set");
            WriteBlockStart();
        }

        public void EndSetter()
        {
            WriteBlockEnd();
            WriteNewline();
        }

        public void BeginIndexer(string accessSpecifier, bool isUnsafe)
        {
            NeedsNewline = true;
            WriteIndented(accessSpecifier);
            Write(' ');
            if (isUnsafe)
            {
                Write("unsafe ");
            }
        }

        public void WriteIndexer(string typeName)
        {
            Write(typeName);
            Write(" this");
        }

        public void BeginIndexerParameters()
        {
            Write('[');
        }

        public void EndIndexerParameters()
        {
            Write(']');
        }

        public void EndIndexer()
        {
            // nop, used only by XML
        }

        public void BeginDereference()
        {
            Write('&');
        }

        public void EndDereference()
        {
            // nop, used only by XML
        }
    }
}
