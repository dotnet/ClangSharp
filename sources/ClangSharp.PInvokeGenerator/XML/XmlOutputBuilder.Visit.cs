// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.XML
{
    internal partial class XmlOutputBuilder
    {
        public void WriteCustomAttribute(string attribute)
            => _sb.Append($"<attribute>{attribute}</attribute>\n");
        public void WriteIid(string iidName, string iidValue)
        {
            _ = _sb.Append("<iid name=\"");
            _ = _sb.Append(iidName);
            _ = _sb.Append("\" value=\"");
            _ = _sb.Append(iidValue);
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
