namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 u\"Test\0\\\r\n\t\"\"")]
        public const string MyMacro1 = "Test\0\\\r\n\t\"";
    }
}
