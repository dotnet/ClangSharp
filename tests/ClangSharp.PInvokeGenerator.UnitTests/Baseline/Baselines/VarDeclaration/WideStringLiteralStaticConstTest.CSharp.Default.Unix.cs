using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("const wchar_t[11]")]
        public static ReadOnlySpan<uint> MyConst1 => [0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000];

        [NativeTypeName("const wchar_t *")]
        public static uint[] MyConst2 = [0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000];

        [NativeTypeName("const wchar_t *const")]
        public static ReadOnlySpan<uint> MyConst3 => [0x00000054, 0x00000065, 0x00000073, 0x00000074, 0x00000000, 0x0000005C, 0x0000000D, 0x0000000A, 0x00000009, 0x00000022, 0x00000000];
    }
}
