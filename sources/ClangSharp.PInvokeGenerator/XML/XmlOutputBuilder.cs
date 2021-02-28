using ClangSharp.Abstractions;

namespace ClangSharp.XML
{
    public partial class XmlOutputBuilder : IOutputBuilder
    {
        public void WriteCustomAttribute(string attribute)
            => _sb.AppendLine($"<attribute>{attribute}</attribute>");
    }
}
