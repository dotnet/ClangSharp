// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.XML
{
    internal partial class XmlOutputBuilder
    {
        public void WriteCustomAttribute(string attribute, Action callback = null)
        {
            _ = _sb.Append($"<attribute>{attribute}");
            callback?.Invoke();
            _ = _sb.Append("</attribute >\n");
        }

        public void WriteIid(string name, Guid value)
        {
            var valueString = value.ToString("X").ToUpperInvariant().Replace("{", "").Replace("}", "").Replace('X', 'x').Replace(",", ", ");
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
}
