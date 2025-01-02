namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define ADDRESS_IN_USE TESTRESULT_FROM_WIN32(10048)")]
        public const int ADDRESS_IN_USE = unchecked((int)(10048) <= 0 ? ((int)(10048)) : ((int)(((10048) & 0x0000FFFF) | (7 << 16) | 0x80000000)));
    }
}
