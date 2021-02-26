using System.Text;

namespace ClangSharp.XML
{
    public partial class XmlOutputBuilder
    {
        private StringBuilder _sb = new();
        public void BeginInnerValue() => _sb.Append("<value>");
        public void EndInnerValue() => _sb.Append("</value>");

        public void BeginInnerCast() => _sb.Append("<cast>");
        public void WriteCastType(string targetTypeName) => _sb.Append(targetTypeName);
        public void EndInnerCast() => _sb.Append("</cast>");

        public void BeginUnchecked() => _sb.Append("<unchecked>");
        public void EndUnchecked() => _sb.Append("</unchecked>");

        public void BeginConstant(string accessSpecifier, string typeName, string escapedName, bool isAnonymousEnum)
        {
            if (isAnonymousEnum)
            {
                _sb.Append($"<constant name=\"{escapedName}\" access=\"{accessSpecifier}\">");
            }
            else
            {
                _sb.Append($"<enumerator name=\"{escapedName}\" access=\"{accessSpecifier}\">");
            }

            _sb.Append($"<type>{typeName}</type>");
        }

        public void BeginConstantValue() => _sb.Append("<value>");
        public void WriteConstantValue(long value) => _sb.Append(value);
        public void WriteConstantValue(ulong value) => _sb.Append(value);
        public void EndConstantValue() => _sb.Append("</value>");
        public void EndConstant(bool isAnonymousEnum) => _sb.Append(isAnonymousEnum ? "</constant>" : "</enumerator>");

        public void BeginEnum(string accessSpecifier, string typeName, string escapedName)
            => _sb.Append($"<enumeration name=\"{escapedName}\" access=\"{accessSpecifier}\"><type>{typeName}</type>");

        public void EndEnum() => _sb.Append("</enumeration>");

        public void BeginField(string accessSpecifier, string nativeTypeName, string escapedName, int? offset)
        {
            _sb.Append($"<field name=\"{escapedName}\" access=\"{accessSpecifier}\"");
            if (offset is not null)
            {
                _sb.Append($" offset=\"{offset}\"");
            }

            _sb.Append(" />");
            _sb.Append($"<type native=\"{nativeTypeName}\"");
        }

        public void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count)
            => _sb.Append($">{typeName}</type><fixed name=\"{fixedName}\" count=\"{count}\" />");

        public void WriteRegularField(string typeName, string escapedName) => _sb.Append($">{typeName}</type>");
        public void EndField() => _sb.Append("</field>");
    }
}
