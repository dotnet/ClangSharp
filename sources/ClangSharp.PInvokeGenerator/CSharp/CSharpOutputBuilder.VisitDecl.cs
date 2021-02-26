using ClangSharp.Abstractions;

namespace ClangSharp.CSharp
{
    public partial class CSharpOutputBuilder : IOutputBuilder
    {
        public void BeginInnerValue() => Write('(');
        public void EndInnerValue() => Write(')');

        public void BeginInnerCast() => Write('(');
        public void WriteCastType(string targetTypeName) => Write(targetTypeName);
        public void EndInnerCast() => Write('(');

        public void BeginUnchecked() => Write("unchecked");
        public void EndUnchecked()
        {
            // nop, used only by XML
        }

        public void BeginConstant(string accessSpecifier, string typeName, string escapedName, bool isAnonymousEnum)
        {
            WriteIndentation();

            if (isAnonymousEnum)
            {
                Write(accessSpecifier);
                Write(" const ");
                Write(typeName);
                Write(' ');
            }

            Write(escapedName);
        }

        public void BeginConstantValue()
        {
            Write(" = ");
        }

        public void WriteConstantValue(long value) => Write(value);
        public void WriteConstantValue(ulong value) => Write(value);

        public void EndConstantValue()
        {
            // nop, used only by the XML backend
        }

        public void EndConstant(bool isAnonymousEnum) => WriteLine(isAnonymousEnum ? ';' : ',');

        public void BeginEnum(string accessSpecifier, string typeName, string escapedName)
        {
            WriteIndented(accessSpecifier);
            Write(" enum ");
            Write(escapedName);

            if (!typeName.Equals("int"))
            {
                Write(" : ");
                Write(typeName);
            }

            NeedsNewline = true;
            WriteBlockStart();
        }

        public void EndEnum() => WriteBlockEnd();

        public void BeginField(string accessSpecifier, string nativeTypeName, string escapedName, int? offset)
        {
            if (offset is not null)
            {
                WriteIndentedLine($"[FieldOffset({offset})]");
            }
            AddNativeTypeNameAttribute(nativeTypeName);

            WriteIndented(accessSpecifier);
            Write(' ');

            if (NeedsNewKeyword(escapedName))
            {
                Write("new ");
            }
        }

        public void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count)
        {
            if (IsSupportedFixedSizedBufferType(typeName))
            {
                Write("fixed ");
                Write(typeName);
                Write(' ');
                Write(escapedName);
                Write('[');
                Write(count);
                Write(']');
            }
            else
            {
                Write(fixedName);
                Write(' ');
                Write(escapedName);
            }
        }

        public void WriteRegularField(string typeName, string escapedName)
        {
            Write(typeName);
            Write(' ');
            Write(escapedName);
        }

        public void EndField()
        {
            WriteSemicolon();
            WriteNewline();
        }
    }
}
