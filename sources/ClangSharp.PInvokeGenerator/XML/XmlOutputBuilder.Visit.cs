// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.XML
{
    internal partial class XmlOutputBuilder
    {
        public void WriteCustomAttribute(string attribute)
            => _sb.Append($"<attribute>{attribute}</attribute>\n");
        public void WriteIid(string iidName, string iidValue)
        {
            _sb.Append("<iid name=\"");
            _sb.Append(iidName);
            _sb.Append("\" value=\"");
            _sb.Append(iidValue);
            _sb.Append("\" />");
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
