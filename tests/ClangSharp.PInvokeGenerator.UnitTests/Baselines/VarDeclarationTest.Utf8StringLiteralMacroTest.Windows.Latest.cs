using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 \"Test\0\\\r\n\t\"\"")]
        public static ReadOnlySpan<byte> MyMacro1 => "Test\0\\\r\n\t\""u8;
    }
}
