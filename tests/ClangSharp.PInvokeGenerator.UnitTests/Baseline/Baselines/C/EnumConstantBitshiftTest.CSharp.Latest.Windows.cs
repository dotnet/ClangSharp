using static ClangSharp.Test.Bits;

namespace ClangSharp.Test
{
    public enum Bits
    {
        BIT_A,
        BIT_B,
    }

    public static partial class Methods
    {
        [NativeTypeName("const unsigned int")]
        public const uint FlagA = 1U << (int)(BIT_A);

        [NativeTypeName("const unsigned int")]
        public const uint FlagB = 1U << (int)(BIT_B);

        [NativeTypeName("const unsigned int")]
        public const uint FlagC = 1U << (int)((BIT_B));

        [NativeTypeName("const unsigned int")]
        public const uint FlagD = 2U >> (int)(BIT_A);

        [NativeTypeName("const unsigned int")]
        public const uint FlagE = 2U >> (int)(BIT_B);

        public static void ShiftAssign([NativeTypeName("unsigned int")] uint value)
        {
            value <<= (int)(BIT_A);
            value <<= (int)(BIT_B);
            value >>= (int)(BIT_A);
            value >>= (int)(BIT_B);
        }

        [NativeTypeName("#define FLAG_A (1U << BIT_A)")]
        public const uint FLAG_A = (1U << (int)(BIT_A));

        [NativeTypeName("#define FLAG_B (1U << (BIT_B))")]
        public const uint FLAG_B = (1U << (int)(BIT_B));
    }
}
