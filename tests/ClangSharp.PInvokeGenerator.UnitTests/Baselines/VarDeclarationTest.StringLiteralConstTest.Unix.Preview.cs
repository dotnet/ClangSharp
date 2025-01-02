using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("const char[11]")]
        public static ReadOnlySpan<byte> MyConst1 => "Test\0\\\r\n\t\""u8;

        [NativeTypeName("const char *")]
        public static byte[] MyConst2 = "Test\0\\\r\n\t\""u8.ToArray();

        [NativeTypeName("const char *const")]
        public static ReadOnlySpan<byte> MyConst3 => "Test\0\\\r\n\t\""u8;
    }
}
