using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 \"Test\0\\\r\n\t\"\"")]
        public static ReadOnlySpan<byte> MyMacro1 => new byte[] { 0x54, 0x65, 0x73, 0x74, 0x00, 0x5C, 0x0D, 0x0A, 0x09, 0x22, 0x00 };
    }
}
