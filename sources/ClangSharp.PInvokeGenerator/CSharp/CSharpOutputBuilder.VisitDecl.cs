// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using ClangSharp.Abstractions;
using ClangSharp.Interop;

namespace ClangSharp.CSharp
{
    internal partial class CSharpOutputBuilder : IOutputBuilder
    {
        public void BeginInnerValue() => Write('(');
        public void EndInnerValue() => Write(')');

        public void BeginInnerCast() => Write('(');
        public void WriteCastType(string targetTypeName) => Write(targetTypeName);
        public void EndInnerCast() => Write(')');

        public void BeginUnchecked() => Write("unchecked");
        public void EndUnchecked()
        {
            // nop, used only by XML
        }

        public void BeginConstant(in ConstantDesc desc)
        {
            if (desc.NativeTypeName is not null)
            {
                AddNativeTypeNameAttribute(desc.NativeTypeName);
            }

            if (desc.Location is {} location)
                WriteSourceLocation(location, false);

            WriteIndentation();

            if ((desc.Kind & ConstantKind.PrimitiveConstant) != 0)
            {
                Write(desc.AccessSpecifier.AsString());
                Write(" const ");
                Write(desc.TypeName);
                Write(' ');
            }
            else if ((desc.Kind & ConstantKind.NonPrimitiveConstant) != 0)
            {
                Write(desc.AccessSpecifier.AsString());
                Write(" static ");
                if ((desc.Kind & ConstantKind.ReadOnly) != 0)
                {
                    Write("readonly ");
                }

                Write(desc.TypeName);
                Write(' ');
            }

            Write(desc.EscapedName);
        }

        public void BeginConstantValue(bool isGetOnlyProperty = false) => Write(isGetOnlyProperty ? " => " : " = ");

        public void WriteConstantValue(long value) => Write(value);
        public void WriteConstantValue(ulong value) => Write(value);

        public void EndConstantValue()
        {
            // nop, used only by the XML backend
        }

        public void EndConstant(bool isConstant) => WriteLine(isConstant ? ';' : ',');

        public void BeginEnum(in EnumDesc desc)
        {
            if (desc.NativeType is not null)
            {
                AddNativeTypeNameAttribute(desc.NativeType);
            }

            if (desc.Location is {} location)
                WriteSourceLocation(location, false);

            WriteIndented(desc.AccessSpecifier.AsString());
            Write(" enum ");
            Write(desc.EscapedName);

            if (!desc.TypeName.Equals("int"))
            {
                Write(" : ");
                Write(desc.TypeName);
            }

            NeedsNewline = true;
            WriteBlockStart();
        }

        public void EndEnum() => WriteBlockEnd();

        public void BeginField(in FieldDesc desc)
        {
            if (desc.Offset is not null)
            {
                WriteIndentedLine($"[FieldOffset({desc.Offset})]");
            }

            if (desc.NativeTypeName is not null)
            {
                AddNativeTypeNameAttribute(desc.NativeTypeName);
            }

            if (desc.Location is {} location)
                WriteSourceLocation(location, false);

            WriteIndented(desc.AccessSpecifier.AsString());
            Write(' ');

            if (desc.NeedsNewKeyword)
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

        public void EndField(bool isBodyless = true)
        {
            if (isBodyless)
            {
                WriteSemicolon();
                WriteNewline();
                NeedsNewline = true;
            }
        }

        public void BeginFunctionOrDelegate<TCustomAttrGeneratorData>(in FunctionOrDelegateDesc<TCustomAttrGeneratorData> desc, ref bool isMethodClassUnsafe)
        {
            desc.WriteCustomAttrs(desc.CustomAttrGeneratorData);

            if (desc.IsVirtual)
            {
                Debug.Assert(!desc.HasFnPtrCodeGen);

                AddUsingDirective("System.Runtime.InteropServices");
                WriteIndented("[UnmanagedFunctionPointer");

                if (desc.CallingConvention != CallingConvention.Winapi)
                {
                    Write("(CallingConvention.");
                    Write(desc.CallingConvention);
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

                if (desc.CallingConvention != CallingConvention.Winapi)
                {
                    Write("CallingConvention = CallingConvention.");
                    Write(desc.CallingConvention);
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
                    Write(", SetLastError = true");
                }
                WriteLine(")]");
            }

            if (desc.Location is {} location)
                WriteSourceLocation(location, false);

            if (desc.IsAggressivelyInlined)
            {
                AddUsingDirective("System.Runtime.CompilerServices");
                WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            }

            var vtblIndex = desc.VtblIndex ?? -1;

            if (vtblIndex != -1)
            {
                AddVtblIndexAttribute(vtblIndex);
            }

            if (desc.NativeTypeName is not null)
            {
                AddNativeTypeNameAttribute(desc.NativeTypeName, attributePrefix: "return: ");
            }

            WriteIndented(desc.AccessSpecifier.AsString());

            if (!desc.IsMemberFunction)
            {
                if (desc.IsVirtual)
                {
                    if (desc.IsUnsafe && !desc.IsCxxRecordCtxUnsafe)
                    {
                        Write(" unsafe");
                    }
                    Write(" delegate");
                }
                else if (desc.IsStatic ?? (desc.IsDllImport || !desc.IsCxx))
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

            if (!desc.IsCxxConstructor)
            {
                Write(desc.ReturnType);
                Write(' ');
            }
        }

        private void WriteSourceLocation(CXSourceLocation location, bool inline)
        {
            if (!_writeSourceLocation)
                return;

            if (!inline)
                WriteIndentation();

            Write("[SourceLocation(\"");
            location.GetFileLocation(out var file, out var line, out var column, out _);
            Write(PInvokeGenerator.EscapeString(file.Name.ToString()));
            Write("\", ");
            Write(line);
            Write(", ");
            Write(column);
            Write(")]");

            if (!inline)
                WriteNewline();
            else
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

            if (info.Location is {} location)
                WriteSourceLocation(location, true);

            _customAttrIsForParameter = true;
            info.WriteCustomAttrs(info.CustomAttrGeneratorData);
            _customAttrIsForParameter = false;
            Write(info.Type);
            Write(' ');
            Write(info.Name);
        }

        public void BeginParameterDefault() => Write(" = ");

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

        public void EndFunctionInnerPrototype() => Write(')');

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

        public void BeginInnerFunctionBody() => WriteIndentation();

        public void EndInnerFunctionBody()
        {
            // nop, used only by XML
        }

        public void EndBody(bool isExpressionBody = false)
        {
            WriteSemicolonIfNeeded();
            WriteNewlineIfNeeded();
            if (!isExpressionBody)
            {
                WriteBlockEnd();
            }
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
            if (info.LayoutAttribute is { } attribute)
            {
                AddUsingDirective("System.Runtime.InteropServices");
                WriteIndented("[StructLayout(LayoutKind.");
                Write(attribute.Value);
                if (attribute.Pack != default)
                {
                    Write(", Pack = ");
                    Write(attribute.Pack);
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

            if (info.Location is {} location)
                WriteSourceLocation(location, false);

            WriteIndented(info.AccessSpecifier.AsString());
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

        public void EmitCompatibleCodeSupport() => AddUsingDirective("System.Runtime.CompilerServices");

        public void EmitFnPtrSupport()
        {
            AddUsingDirective("System");
            AddUsingDirective("System.Runtime.InteropServices");
        }

        public void EmitSystemSupport() => AddUsingDirective("System");

        public void EndStruct() => WriteBlockEnd();

        public void EndExplicitVtbl() => WriteBlockEnd();

        public CSharpOutputBuilder BeginCSharpCode() =>
            // just write directly to this buffer
            this;

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

        public void EndGetter() => WriteBlockEnd();

        public void BeginSetter(bool aggressivelyInlined)
        {
            WriteNewline();
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
            NeedsNewline = false;
        }

        public void BeginIndexer(AccessSpecifier accessSpecifier, bool isUnsafe)
        {
            NeedsNewline = true;
            WriteIndented(accessSpecifier.AsString());
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

        public void BeginIndexerParameters() => Write('[');

        public void EndIndexerParameters() => Write(']');

        public void EndIndexer() => NeedsNewline = true;

        public void BeginDereference() => Write('&');

        public void EndDereference()
        {
            // nop, used only by XML
        }
    }
}
