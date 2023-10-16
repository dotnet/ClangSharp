// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Globalization;

namespace ClangSharp.XML;

internal partial class XmlOutputBuilder
{
    public void WriteCustomAttribute(string attribute, Action? callback = null)
    {
        _ = _sb.Append(CultureInfo.InvariantCulture, $"<attribute>{attribute}");
        callback?.Invoke();
        _ = _sb.Append("</attribute >\n");
    }

    public void WriteIid(string name, Guid value)
    {
        var valueString = value.ToString("X", CultureInfo.InvariantCulture).ToUpperInvariant()
                               .Replace("{", "", StringComparison.Ordinal)
                               .Replace("}", "", StringComparison.Ordinal)
                               .Replace('X', 'x')
                               .Replace(",", ", ", StringComparison.Ordinal);
        WriteIid(name, valueString);
    }

    public void WriteIid(string name, string value)
    {
        _ = _sb.Append("<iid name=\"");
        _ = _sb.Append(name);
        _ = _sb.Append("\" value=\"");
        _ = _sb.Append(value);
        _ = _sb.Append("\" />");
    }

    public void WriteDivider(bool force = false)
    {
        // nop, used only by C#
    }

    public void SuppressDivider()
    {
        // nop, used only by C#
    }

    public void EmitUsingDirective(string directive)
    {
        // nop, used only by C#
    }
}
