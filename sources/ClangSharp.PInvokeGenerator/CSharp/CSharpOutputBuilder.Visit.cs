namespace ClangSharp.CSharp
{
    internal partial class CSharpOutputBuilder
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

        public void WriteDivider(bool force = false)
        {
            if (force)
            {
                WriteNewline();
            }
            else
            {
                NeedsNewline = true;
            }
        }

        public void SuppressDivider()
        {
            NeedsNewline = false;
        }

        public void WriteIid(string iidName, string iidValue)
        {
            AddUsingDirective("System");
            WriteIndented("public static readonly Guid ");
            Write(iidName);
            Write(" = new Guid(");
            Write(iidValue);
            Write(")");
            WriteSemicolon();
            WriteNewline();
        }

        public void EmitUsingDirective(string directive) => AddUsingDirective(directive);
    }
}
