namespace ClangSharp.XML
{
    internal partial class XmlOutputBuilder
    {
        public void WriteCustomAttribute(string attribute)
            => _sb.AppendLine($"<attribute>{attribute}</attribute>");
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
