namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("const wchar_t[11]")]
        public const string MyConst1 = "Test\0\\\r\n\t\"";

        [NativeTypeName("const wchar_t *")]
        public static string MyConst2 = "Test\0\\\r\n\t\"";

        [NativeTypeName("const wchar_t *const")]
        public const string MyConst3 = "Test\0\\\r\n\t\"";
    }
}
