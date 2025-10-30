// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using ClangSharp.Abstractions;
using ClangSharp.Interop;

namespace ClangSharp.CSharp;

internal partial class CSharpOutputBuilder : IOutputBuilder
{
    public void BeginInnerValue() => Write('(');
    public void EndInnerValue() => Write(')');

    public void BeginInnerCast() => Write('(');
    public void WriteCastType(string targetTypeName) => Write(targetTypeName);
    public void EndInnerCast() => Write(')');

    public void BeginUnchecked()
    {
        Debug.Assert(!IsUncheckedContext);
        Write("unchecked");
        IsUncheckedContext = true;
    }
    public void EndUnchecked()
    {
        Debug.Assert(IsUncheckedContext);
        IsUncheckedContext = false;
    }

    public void BeginValue(in ValueDesc desc)
    {
        if (_generator.Config.GenerateDocIncludes && (desc.Kind == ValueKind.Enumerator))
        {
            WriteIndented("/// <include file='");
            Write(desc.ParentName);
            Write(".xml' path='doc/member[@name=\"");
            Write(desc.ParentName);
            Write('.');
            Write(desc.EscapedName);
            WriteLine("\"]/*' />");
        }

        if (desc.NativeTypeName is not null)
        {
            AddNativeTypeNameAttribute(desc.NativeTypeName);
        }

        if (desc.Location is { } location)
        {
            WriteSourceLocation(location, false);
        }

        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);

        var isProperty = false;
        var isExpressionBody = false;

        if (desc.Kind == ValueKind.String)
        {
            isExpressionBody = true;
        }
        else if (desc.IsConstant)
        {
            if (_generator.Config.GenerateUnmanagedConstants && (desc.Kind != ValueKind.Enumerator))
            {
                if (desc.IsCopy)
                {
                    isExpressionBody = true;
                }
                else if (desc.Kind == ValueKind.Unmanaged)
                {
                    isProperty = true;
                }
            }
        }

        WriteIndentation();

        if (desc.Kind == ValueKind.Primitive)
        {
            Write(GetAccessSpecifierString(desc.AccessSpecifier, isNested: true));

            if (desc.IsConstant)
            {
                if (desc.IsCopy)
                {
                    Write(" static ");

                    if (!_generator.Config.GenerateUnmanagedConstants)
                    {
                        Write("readonly ");
                    }
                }
                else
                {
                    Write(" const ");
                }
            }
            else
            {
                Write(" static ");
            }

            Write(desc.TypeName);
            Write(' ');
        }
        else if (desc.Kind == ValueKind.Unmanaged)
        {
            Write(GetAccessSpecifierString(desc.AccessSpecifier, isNested: true));
            Write(" static ");

            if (_generator.Config.GenerateUnmanagedConstants && desc.IsConstant)
            {
                if (desc.IsArray)
                {
                    Write("ReadOnlySpan<");
                }
                else
                {
                    Write("ref readonly ");
                }

                Write(desc.TypeName);

                if (desc.IsArray)
                {
                    Write('>');
                }
            }
            else
            {
                if (desc.IsConstant)
                {
                    Write("readonly ");
                }
                Write(desc.TypeName);
            }

            Write(' ');
        }
        else if (desc.Kind == ValueKind.String)
        {
            Write(GetAccessSpecifierString(desc.AccessSpecifier, isNested: true));
            Write(" static ");

            if (desc.TypeName is "byte[]" or "uint[]")
            {
                if (desc.IsConstant)
                {
                    Write("readonly ");
                }
                isExpressionBody = false;
            }

            Write(desc.TypeName);
            Write(' ');
        }
        else if (desc.Kind == ValueKind.GuidMember)
        {
            Write("static ");
            Write(desc.TypeName);
            Write(' ');
            isExpressionBody = true;
        }

        Write(desc.EscapedName);

        if (desc.HasInitializer)
        {
            if (isExpressionBody)
            {
                Write(" => ");
            }
            else if (isProperty)
            {
                WriteNewline();
                WriteBlockStart();
                BeginGetter(desc.IsConstant && _generator.Config.GenerateAggressiveInlining, isReadOnly: false);
            }
            else
            {
                Write(" = ");
            }
        }
    }

    public void WriteConstantValue(long value) => Write(value);
    public void WriteConstantValue(ulong value) => Write(value);

    public void EndValue(in ValueDesc desc)
    {
        switch (desc.Kind)
        {
            case ValueKind.Primitive:
            {
                WriteLine(';');
                break;
            }

            case ValueKind.Enumerator:
            {
                WriteLine(',');

                if (_generator.Config.GenerateDocIncludes)
                {
                    NeedsNewline = true;
                }
                break;
            }

            case ValueKind.Unmanaged:
            {
                if (desc.IsConstant)
                {
                    if (_generator.Config.GenerateUnmanagedConstants && !desc.IsCopy)
                    {
                        EndGetter();
                        WriteBlockEnd();
                    }
                    else
                    {
                        WriteLine(';');
                    }
                }
                break;
            }

            case ValueKind.String:
            {
                WriteLine(';');
                break;
            }

            case ValueKind.GuidMember:
            {
                WriteLine(';');
                NeedsNewline = true;
                break;
            }
        }
    }

    public void BeginEnum(in EnumDesc desc)
    {
        if (_generator.Config.GenerateDocIncludes)
        {
            WriteIndented("/// <include file='");
            Write(desc.EscapedName);
            Write(".xml' path='doc/member[@name=\"");
            Write(desc.EscapedName);
            WriteLine("\"]/*' />");
        }

        if (desc.NativeType is not null)
        {
            AddNativeTypeNameAttribute(desc.NativeType);
        }

        if (desc.Location is {} location)
        {
            WriteSourceLocation(location, false);
        }

        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);

        WriteIndented(GetAccessSpecifierString(desc.AccessSpecifier, desc.IsNested));
        Write(" enum ");
        Write(desc.EscapedName);

        if (!desc.TypeName.Equals("int", StringComparison.Ordinal))
        {
            Write(" : ");
            Write(desc.TypeName);
        }

        NeedsNewline = true;
        WriteBlockStart();
    }

    public void EndEnum(in EnumDesc desc) => WriteBlockEnd();

    public void BeginField(in FieldDesc desc)
    {
        if (_generator.Config.GenerateDocIncludes && !string.IsNullOrWhiteSpace(desc.ParentName))
        {
            WriteIndented("/// <include file='");
            Write(desc.ParentName);
            Write(".xml' path='doc/member[@name=\"");
            Write(desc.ParentName);
            Write('.');
            Write(desc.EscapedName);
            WriteLine("\"]/*' />");
        }

        if (desc.Offset is not null)
        {
            WriteIndentedLine($"[FieldOffset({desc.Offset})]");
        }

        if (desc.NativeTypeName is not null)
        {
            AddNativeTypeNameAttribute(desc.NativeTypeName);
        }

        if (desc.Location is { } location)
        {
            WriteSourceLocation(location, false);
        }

        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);

        if (desc.NeedsUnscopedRef)
        {
            WriteCustomAttribute("UnscopedRef");
        }

        WriteIndented(GetAccessSpecifierString(desc.AccessSpecifier, isNested: true));
        Write(' ');

        if (desc.NeedsNewKeyword)
        {
            Write("new ");
        }
    }

    public void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count)
    {
        if (_generator.IsSupportedFixedSizedBufferType(typeName))
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

    public void EndField(in FieldDesc desc)
    {
        if (!desc.HasBody)
        {
            WriteSemicolon();
            WriteNewline();
            NeedsNewline = true;
        }
    }

    public void BeginFunctionOrDelegate(in FunctionOrDelegateDesc desc, ref bool isMethodClassUnsafe)
    {
        if (_generator.Config.GenerateDocIncludes && !string.IsNullOrEmpty(desc.ParentName))
        {
            if (desc.IsInherited)
            {
                WriteIndented("/// <inheritdoc cref=\"");
                Write(desc.ParentName);
                Write('.');
                Write(desc.EscapedName);
                if (desc.ParameterTypes is not null)
                {
                    Write('(');
                    Write(string.Join(", ", desc.ParameterTypes));
                    Write(')');
                }
                WriteLine("\" />");
            }
            else
            {
                WriteIndented("/// <include file='");
                Write(desc.ParentName);
                Write(".xml' path='doc/member[@name=\"");
                Write(desc.ParentName);
                Write('.');
                Write(desc.EscapedName);
                WriteLine("\"]/*' />");
            }
        }

        if (desc.IsVirtual)
        {
            Debug.Assert(!desc.HasFnPtrCodeGen);

            AddUsingDirective("System.Runtime.InteropServices");
            WriteIndented("[UnmanagedFunctionPointer");

            if (desc.CallingConvention != CallConv.Winapi)
            {
                Write("(CallingConvention.");
                Write(desc.CallingConvention);
                Write(')');
            }

            WriteLine(']');
        }
        else if (desc.IsDllImport && !desc.IsManualImport)
        {
            AddUsingDirective("System.Runtime.InteropServices");

            WriteIndented("[DllImport(");

            Write('"');
            Write(desc.LibraryPath);
            Write('"');

            Write(", ");

            if (desc.CallingConvention != CallConv.Winapi)
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

            if (desc.SetLastError && !_generator.Config.GenerateSetsLastSystemErrorAttribute)
            {
                Write(", SetLastError = true");
            }

            WriteLine(")]");
        }

        if (desc.Location is {} location)
        {
            WriteSourceLocation(location, false);
        }

        if (desc.SetLastError && _generator.Config.GenerateSetsLastSystemErrorAttribute)
        {
            WriteIndentedLine("[SetsLastSystemError]");
        }

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

        if ((desc.NativeTypeName is not null) && !desc.IsManualImport)
        {
            AddNativeTypeNameAttribute(desc.NativeTypeName, attributePrefix: "return: ");
        }

        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);

        if (desc.NeedsUnscopedRef)
        {
            WriteCustomAttribute("UnscopedRef");
        }

        if (_isInMarkerInterface)
        {
            WriteIndentation();
        }
        else
        {
            WriteIndented(GetAccessSpecifierString(desc.AccessSpecifier, isNested: true));
            Write(' ');
        }

        if (!desc.IsMemberFunction)
        {
            if (desc.IsVirtual)
            {
                if (desc.IsUnsafe && !desc.IsCxxRecordCtxUnsafe)
                {
                    Write("unsafe ");
                }
                Write("delegate ");
            }
            else if ((desc.IsStatic ?? (desc.IsDllImport || !desc.IsCxx)) && !desc.IsManualImport)
            {
                Write("static ");

                if (desc.IsDllImport)
                {
                    Write("extern ");
                }
            }
        }

        if (!desc.IsVirtual)
        {
            if (desc.NeedsNewKeyword)
            {
                Write("new ");
            }

            if (desc.IsReadOnly)
            {
                Write("readonly ");
            }

            if (desc.IsUnsafe)
            {
                if (!desc.IsCtxCxxRecord)
                {
                    isMethodClassUnsafe = true;
                }
                else if (!desc.IsCxxRecordCtxUnsafe)
                {
                    Write("unsafe ");
                }
            }
        }

        if (!desc.IsCxxConstructor && !desc.IsManualImport)
        {
            Write(desc.ReturnType);
            Write(' ');
        }
    }

    private void WriteSourceLocation(CXSourceLocation location, bool inline)
    {
        if (!_writeSourceLocation)
        {
            return;
        }

        if (!inline)
        {
            WriteIndentation();
        }

        Write("[SourceLocation(\"");
        location.GetFileLocation(out var file, out var line, out var column, out _);
        Write(PInvokeGenerator.EscapeString(file.Name.ToString()));
        Write("\", ");
        Write(line);
        Write(", ");
        Write(column);
        Write(")]");

        if (!inline)
        {
            WriteNewline();
        }
        else
        {
            Write(' ');
        }
    }

    public void BeginFunctionInnerPrototype(in FunctionOrDelegateDesc desc)
    {
        if (desc.IsManualImport)
        {
            Write("delegate* unmanaged");

            if (desc.CallingConvention != CallConv.Winapi)
            {
                Write('[');
                Write(desc.CallingConvention);
                Write(']');
            }

            Write('<');
        }
        else
        {
            Write(desc.EscapedName);
            Write('(');
        }
    }

    public void BeginParameter(in ParameterDesc info)
    {
        if (info.IsForManualImport)
        {
            Write(info.Type);
        }
        else
        {
            if (info.NativeTypeName is not null)
            {
                AddNativeTypeNameAttribute(info.NativeTypeName, prefix: "", postfix: " ");
            }

            if (info.CppAttributes is not null)
            {
                AddCppAttributes(info.CppAttributes, prefix: "", postfix: " ");
            }

            if (info.Location is { } location)
            {
                WriteSourceLocation(location, true);
            }

            _customAttrIsForParameter = true;
            info.WriteCustomAttrs?.Invoke(info.CustomAttrGeneratorData);
            _customAttrIsForParameter = false;

            Write(info.Type);
            if (info.Name.Length > 0)
            {
                Write(' ');
                Write(info.Name);
            }
        }
    }

    public void BeginParameterDefault() => Write(" = ");

    public void EndParameterDefault()
    {
        // nop, used only by XML
    }

    public void EndParameter(in ParameterDesc info)
    {
        // nop, used only by XML
    }

    public void WriteParameterSeparator()
    {
        Write(',');
        Write(' ');
    }

    public void EndFunctionInnerPrototype(in FunctionOrDelegateDesc desc)
    {
        if (desc.IsManualImport)
        {
            Write(desc.ReturnType);
            Write('>');
        }
        else
        {
            Write(')');
        }
    }

    public void BeginConstructorInitializer(string memberRefName, string memberInitName)
    {
        WriteIndentation();

        if (memberRefName.Equals(memberInitName, StringComparison.Ordinal))
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

    public void EndFunctionOrDelegate(in FunctionOrDelegateDesc desc)
    {
        if (desc.IsManualImport)
        {
            Write(' ');
            Write(desc.EscapedName);
        }

        if (!desc.HasBody || desc.IsVirtual)
        {
            WriteSemicolon();
            WriteNewline();
        }

        NeedsNewline = true;
    }

    public void BeginStruct(in StructDesc desc)
    {
        if (_generator.Config.GenerateDocIncludes)
        {
            WriteIndented("/// <include file='");
            Write(desc.EscapedName);
            Write(".xml' path='doc/member[@name=\"");
            Write(desc.EscapedName);
            WriteLine("\"]/*' />");
        }

        if (desc.LayoutAttribute is not null)
        {
            AddUsingDirective("System.Runtime.InteropServices");
            WriteIndented("[StructLayout(LayoutKind.");
            Write(desc.LayoutAttribute.Value);

            if (desc.Layout.Pack is not null)
            {
                Write(", Pack = ");
                Write(desc.Layout.Pack);
            }

            WriteLine(")]");
        }

        if (desc.Uuid is not null)
        {
            AddUsingDirective("System.Runtime.InteropServices");

            WriteIndented("[Guid(\"");
            Write(desc.Uuid.Value.ToString("D", CultureInfo.InvariantCulture).ToUpperInvariant());
            WriteLine("\")]");
        }

        if (desc.NativeType is not null)
        {
            AddNativeTypeNameAttribute(desc.NativeType);
        }

        if (desc.NativeInheritance is not null)
        {
            AddNativeInheritanceAttribute(desc.NativeInheritance);
        }

        if (desc.Location is {} location)
        {
            WriteSourceLocation(location, false);
        }

        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);

        WriteIndented(GetAccessSpecifierString(desc.AccessSpecifier, desc.IsNested));
        Write(' ');

        if (desc.IsUnsafe)
        {
            Write("unsafe ");
        }

        Write("partial struct ");
        Write(desc.EscapedName);

        if (_generator.Config.GenerateMarkerInterfaces)
        {
            if (desc.HasVtbl)
            {
                Write(" : ");
                Write(desc.EscapedName);
                Write(".Interface");
            }

            if ((desc.Uuid is not null) && _generator.Config.GenerateGuidMember && !_generator.Config.GenerateCompatibleCode)
            {
                Write(desc.HasVtbl ? ", " : " : ");
                Write("INativeGuid");
            }
        }

        WriteNewline();
        WriteBlockStart();
    }

    public void BeginMarkerInterface(string[]? baseTypeNames)
    {
        WriteIndented("public partial interface Interface");

        if (baseTypeNames is not null)
        {
            Write(" : ");
            Write(baseTypeNames[0]);
            Write(".Interface");

            for (var i = 1; i < baseTypeNames.Length; i++)
            {
                Write(", ");
                Write(baseTypeNames[i]);
                Write(".Interface");
            }
        }

        WriteNewline();
        WriteBlockStart();
        _isInMarkerInterface = true;
    }

    public void BeginExplicitVtbl()
    {
        WriteIndented("public partial struct Vtbl");

        if (_generator.Config.GenerateMarkerInterfaces && !_generator.Config.ExcludeFnptrCodegen)
        {
            WriteLine("<TSelf>");
            IncreaseIndentation();
            WriteIndentedLine("where TSelf : unmanaged, Interface");
            DecreaseIndentation();
        }
        else
        {
            WriteNewline();
        }

        WriteBlockStart();
    }

    public void EmitCompatibleCodeSupport() => AddUsingDirective("System.Runtime.CompilerServices");

    public void EmitFnPtrSupport()
    {
        AddUsingDirective("System");
        AddUsingDirective("System.Runtime.InteropServices");
    }

    public void EmitSystemSupport() => AddUsingDirective("System");

    public void EndStruct(in StructDesc desc) => WriteBlockEnd();

    public void EndMarkerInterface()
    {
        _isInMarkerInterface = false;
        WriteBlockEnd();
    }

    public void EndExplicitVtbl() => WriteBlockEnd();

    public CSharpOutputBuilder BeginCSharpCode() =>
        // just write directly to this buffer
        this;

    public void EndCSharpCode(CSharpOutputBuilder output)
    {
        // nop, used only by XML
    }

    public void BeginGetter(bool aggressivelyInlined, bool isReadOnly)
    {
        if (aggressivelyInlined)
        {
            AddUsingDirective("System.Runtime.CompilerServices");
            WriteIndentedLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        }

        WriteIndentedLine(isReadOnly ? "readonly get" : "get");
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

    public void BeginIndexer(AccessSpecifier accessSpecifier, bool isUnsafe, bool needsUnscopedRef)
    {
        NeedsNewline = true;

        if (needsUnscopedRef)
        {
            WriteCustomAttribute("UnscopedRef");
        }

        WriteIndented(GetAccessSpecifierString(accessSpecifier, isNested: true));
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

    private static string GetAccessSpecifierString(AccessSpecifier accessSpecifier, bool isNested)
    {
        switch (accessSpecifier)
        {
            case AccessSpecifier.Private:
            {
                // Non-nested members can only be public or internal
                // and C# defaults to internal when no specifier is given

                if (!isNested)
                {
                    accessSpecifier = AccessSpecifier.Internal;
                }
                break;
            }

            case AccessSpecifier.PrivateProtected:
            {
                // We only generate structs, enums, or static members so protected is invalid
                // fallback to internal to match the non-external visibility of private protected

                accessSpecifier = AccessSpecifier.Internal;
                break;
            }

            case AccessSpecifier.Protected:
            case AccessSpecifier.ProtectedInternal:
            {
                // We only generate structs, enums, or static members so protected is invalid
                // fallback to internal to match the external visibility of protected and protected internal

                accessSpecifier = AccessSpecifier.Public;
                break;
            }
        }

        return accessSpecifier.AsString();
    }
}
