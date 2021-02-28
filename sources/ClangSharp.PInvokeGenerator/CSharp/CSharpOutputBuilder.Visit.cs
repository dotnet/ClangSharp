namespace ClangSharp.CSharp
{
    public partial class CSharpOutputBuilder
    {
        private bool _customAttrIsForParameter = false;
        public void WriteCustomAttribute(string attribute)
        {
            if (attribute.Equals("Flags") || attribute.Equals("Obsolete"))
            {
                AddUsingDirective("System");
            }
            else if (attribute.Equals("EditorBrowsable") || attribute.StartsWith("EditorBrowsable("))
            {
                AddUsingDirective("System.ComponentModel");
            }
            else if (attribute.StartsWith("Guid("))
            {
                AddUsingDirective("System.Runtime.InteropServices");
            }

            if (!_customAttrIsForParameter)
            {
                WriteIndented('[');
                Write(attribute);
                WriteLine(']');
            }
            else
            {
                Write('[');
                Write(attribute);
                Write(']');
                Write(' ');
            }
        }
    }
}
