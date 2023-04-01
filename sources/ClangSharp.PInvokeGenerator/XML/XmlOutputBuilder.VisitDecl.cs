// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;

namespace ClangSharp.XML;

internal partial class XmlOutputBuilder
{
    private readonly StringBuilder _sb = new();
    public void BeginInnerValue() => _sb.Append("<value>");
    public void EndInnerValue() => _sb.Append("</value>");

    public void BeginInnerCast() => _sb.Append("<cast>");
    public void WriteCastType(string targetTypeName) => _sb.Append(targetTypeName);
    public void EndInnerCast() => _sb.Append("</cast>");

    public void BeginUnchecked()
    {
        Debug.Assert(!IsUncheckedContext);
        _ = _sb.Append("<unchecked>");
        IsUncheckedContext = true;
    }

    public void EndUnchecked()
    {
        Debug.Assert(IsUncheckedContext);
        _ = _sb.Append("</unchecked>");
        IsUncheckedContext = false;
    }

    public void BeginValue(in ValueDesc desc)
    {
        if (desc.Kind == ValueKind.Enumerator)
        {
            _ = _sb.Append("<enumerator");
        }
        else if (desc.IsConstant)
        {
            _ = _sb.Append("<constant");
        }
        else
        {
            _ = _sb.Append("<field");
        }

        _ = _sb.Append($" name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier.AsString()}\">");

        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);
        _ = _sb.Append($"<type primitive=\"{desc.Kind == ValueKind.Primitive}\">");
        _ = _sb.Append(EscapeText(desc.TypeName));
        _ = _sb.Append("</type>");

        if (desc.HasInitializer)
        {
            _ = _sb.Append("<value>");
        }
    }

    public void WriteConstantValue(long value) => _sb.Append(value);
    public void WriteConstantValue(ulong value) => _sb.Append(value);

    public void EndValue(in ValueDesc desc)
    {
        if (desc.HasInitializer)
        {
            _ = _sb.Append("</value>");
        }

        if (desc.Kind == ValueKind.Enumerator)
        {
            _ = _sb.Append("</enumerator>");
        }
        else if (desc.IsConstant)
        {
            _ = _sb.Append("</constant>");
        }
        else
        {
            _ = _sb.Append("</field>");
        }
    }

    public void BeginEnum(in EnumDesc desc)
    {
        _ = _sb.Append($"<enumeration name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier.AsString()}\">");
        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);
        _ = _sb.Append($"<type>{desc.TypeName}</type>");
    }

    public void EndEnum(in EnumDesc desc) => _sb.Append("</enumeration>");

    public void BeginField(in FieldDesc desc)
    {
        _ = _sb.Append($"<field name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier.AsString()}\"");
        if (desc.InheritedFrom is not null)
        {
            _ = _sb.Append($" inherited=\"{desc.InheritedFrom}\"");
        }

        if (desc.Offset is not null)
        {
            _ = _sb.Append($" offset=\"{desc.Offset}\"");
        }

        _ = _sb.Append('>');
        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);
        _ = _sb.Append("<type");

        AddNativeTypeNameAttribute(desc.NativeTypeName);
    }

    public void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count)
        => _sb.Append($" count=\"{count}\" fixed=\"{fixedName}\">" +
                      $"{EscapeText(typeName)}</type>");

    public void WriteRegularField(string typeName, string escapedName)
        => _sb.Append($">{EscapeText(typeName)}</type>");
    public void EndField(in FieldDesc desc) => _sb.Append("</field>");
    public void BeginFunctionOrDelegate(in FunctionOrDelegateDesc desc, ref bool isMethodClassUnsafe)
    {
        if (desc.IsVirtual)
        {
            Debug.Assert(!desc.HasFnPtrCodeGen);
            _ = _sb.Append($"<delegate name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier.AsString()}\"");
            if (desc.CallingConvention != CallingConvention.Winapi)
            {
                _ = _sb.Append($" convention=\"{desc.CallingConvention.AsString(false)}\"");
            }
        }
        else if (desc.IsDllImport)
        {
            _ = _sb.Append($"<function name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier.AsString()}\"");
            _ = _sb.Append($" lib=\"{desc.LibraryPath}\"");
            if (desc.CallingConvention != CallingConvention.Winapi)
            {
                _ = _sb.Append($" convention=\"{desc.CallingConvention.AsString(false)}\"");
            }

            if (desc.EntryPoint != desc.EscapedName)
            {
                _ = _sb.Append($" entrypoint=\"{desc.EntryPoint}\"");
            }

            if (desc.SetLastError)
            {
                _ = _sb.Append(" setlasterror=\"true\"");
            }
        }
        else
        {
            _ = _sb.Append($"<function name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier.AsString()}\"");
        }

        if (!desc.IsMemberFunction && (desc.IsStatic ?? (desc.IsDllImport || !desc.IsCxx)))
        {
            _ = _sb.Append(" static=\"true\"");
        }

        if (desc.IsUnsafe)
        {
            _ = _sb.Append(" unsafe=\"true\"");
        }

        var vtblIndex = desc.VtblIndex ?? -1;

        if (vtblIndex != -1)
        {
            _ = _sb.Append($" vtblindex=\"{vtblIndex}\"");
        }

        _ = _sb.Append('>');

        desc.WriteCustomAttrs?.Invoke(desc.CustomAttrGeneratorData);

        _ = _sb.Append("<type");

        AddNativeTypeNameAttribute(desc.NativeTypeName);

        _ = _sb.Append('>');
        _ = _sb.Append(EscapeText(desc.ReturnType));
        _ = _sb.Append("</type>");
    }

    public void BeginFunctionInnerPrototype(in FunctionOrDelegateDesc info)
    {
        // nop, only used in C#
    }

    public void BeginParameter(in ParameterDesc info)
    {
        _ = _sb.Append($"<param name=\"{info.Name}\">");
        info.WriteCustomAttrs?.Invoke(info.CustomAttrGeneratorData);
        _ = _sb.Append("<type>");
        _ = _sb.Append(EscapeText(info.Type));
        _ = _sb.Append("</type>");
    }

    public void BeginParameterDefault() => _ = _sb.Append("<init>");

    public void EndParameterDefault() => _ = _sb.Append("</init>");

    public void EndParameter(in ParameterDesc info) => _ = _sb.Append("</param>");

    public void WriteParameterSeparator()
    {
        // nop, used only in C#
    }

    public void EndFunctionInnerPrototype(in FunctionOrDelegateDesc info)
    {
        // nop, used only in C#
    }

    public void BeginBody(bool isExpressionBody = false)
    {
        // nop, used only by C#
    }

    public void BeginConstructorInitializers()
    {
        // nop, method only exists for consistency and/or future use
    }

    public void BeginConstructorInitializer(string memberRefName, string memberInitName) =>
        // "hint" is the name we're initializing using, but should only be used as a "hint" rather than a definitive
        // value, which is contained within the init block.
        _ = _sb.Append($"<init field=\"{memberRefName}\" hint=\"{memberInitName}\">");

    public void EndConstructorInitializer() => _sb.Append("</init>");

    public void EndConstructorInitializers()
    {
        // nop, method only exists for consistency and/or future use
    }

    public void BeginInnerFunctionBody() => _ = _sb.Append("<body>");

    public void EndInnerFunctionBody() => _ = _sb.Append("</body>");

    public void EndBody(bool isExpressionBody = false)
    {
        // nop, used only by C#
    }

    public void EndFunctionOrDelegate(in FunctionOrDelegateDesc desc)
        => _sb.Append(desc.IsVirtual ? "</delegate>" : "</function>");

    public void BeginStruct(in StructDesc info)
    {
        _ = _sb.Append("<struct name=\"");
        _ = _sb.Append(info.EscapedName);
        _ = _sb.Append("\" access=\"");
        _ = _sb.Append(info.AccessSpecifier.AsString());
        _ = _sb.Append('"');

        AddNativeTypeNameAttribute(info.NativeType);

        if (info.NativeInheritance is not null)
        {
            _ = _sb.Append(" parent=\"");
            _ = _sb.Append(info.NativeInheritance);
            _ = _sb.Append('"');
        }

        if (info.Uuid is not null)
        {
            _ = _sb.Append(" uuid=\"");
            _ = _sb.Append(info.Uuid.Value);
            _ = _sb.Append('"');
        }

        if (info.HasVtbl)
        {
            _ = _sb.Append(" vtbl=\"true\"");
        }

        if (info.IsUnsafe)
        {
            _ = _sb.Append(" unsafe=\"true\"");
        }

        if (info.LayoutAttribute is not null)
        {
            _ = _sb.Append(" layout=\"");
            _ = _sb.Append(info.LayoutAttribute.Value);
            _ = _sb.Append('"');

            if (info.Layout.Pack is not null)
            {
                _ = _sb.Append(" pack=\"");
                _ = _sb.Append(info.Layout.Pack);
                _ = _sb.Append('"');
            }
        }

        _ = _sb.Append('>');
        info.WriteCustomAttrs?.Invoke(info.CustomAttrGeneratorData);
    }
    public void BeginMarkerInterface(string[]? baseTypeNames) => _sb.Append("<interface>");
    public void EndMarkerInterface() => _sb.Append("</interface>");
    public void BeginExplicitVtbl() => _sb.Append("<vtbl>");
    public void EndExplicitVtbl() => _sb.Append("</vtbl>");

    public void EndStruct(in StructDesc info) => _sb.Append("</struct>");

    public void EmitCompatibleCodeSupport()
    {
        // nop, used only by C#
    }

    public void EmitFnPtrSupport()
    {
        // nop, used only by C#
    }

    public void EmitSystemSupport()
    {
        // nop, used only by C#
    }

    public CSharpOutputBuilder BeginCSharpCode()
    {
        _ = _sb.Append("<code>");
        return new CSharpOutputBuilder("__Internal", _config, markerMode: MarkerMode.Xml);
    }

    public void EndCSharpCode(CSharpOutputBuilder output)
    {
        output.WritePendingLine();

        var needsNewline = false;

        foreach (var s in output.Contents)
        {
            if (needsNewline)
            {
                _ = _sb.Append('\n');
            }

            _ = _sb.Append(EscapeText(s).Replace("/*M*/&lt;", "<")
                                    .Replace("/*M*/&gt;", ">"));

            needsNewline = true;
        }

        _ = _sb.Append("</code>");
    }

    public void BeginGetter(bool aggressivelyInlined)
    {
        _ = _sb.Append("<get");
        if (aggressivelyInlined)
        {
            _ = _sb.Append(" inlining=\"aggressive\"");
        }

        _ = _sb.Append('>');
    }

    public void EndGetter() => _ = _sb.Append("</get>");

    public void BeginSetter(bool aggressivelyInlined)
    {
        _ = _sb.Append("<set");
        if (aggressivelyInlined)
        {
            _ = _sb.Append(" inlining=\"aggressive\"");
        }

        _ = _sb.Append('>');
    }

    public void EndSetter() => _ = _sb.Append("</set>");

    public void BeginIndexer(AccessSpecifier accessSpecifier, bool isUnsafe, bool needsUnscopedRef)
    {
        _ = _sb.Append("<indexer access=\"");
        _ = _sb.Append(accessSpecifier.AsString());
        _ = _sb.Append(isUnsafe ? "\" unsafe=\"true\">" : "\">");
    }

    public void WriteIndexer(string typeName)
    {
        _ = _sb.Append("<type>");
        _ = _sb.Append(EscapeText(typeName));
        _ = _sb.Append("</type>");
    }

    public void BeginIndexerParameters()
    {
        // nop, used only by C#
    }

    public void EndIndexerParameters()
    {
        // nop, used only by C#
    }

    public void EndIndexer() => _sb.Append("</indexer>");

    public void BeginDereference() => _sb.Append("<deref>");

    public void EndDereference() => _sb.Append("</deref>");

    private void AddNativeTypeNameAttribute(string? nativeTypeName)
    {
        if (nativeTypeName is null)
        {
            return;
        }

        foreach (var entry in _config.NativeTypeNamesToStrip)
        {
            nativeTypeName = nativeTypeName.Replace(entry, "");
        }

        if (string.IsNullOrWhiteSpace(nativeTypeName))
        {
            return;
        }

        _ = _sb.Append(" native=\"");
        _ = _sb.Append(EscapeText(nativeTypeName));
        _ = _sb.Append('"');
    }
}
