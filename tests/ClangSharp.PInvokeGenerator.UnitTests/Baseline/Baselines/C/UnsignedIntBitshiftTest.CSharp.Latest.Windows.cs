namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("const int")]
        public const int Signed = 1;

        [NativeTypeName("const long")]
        public const int SignedLong = 1;

        [NativeTypeName("const unsigned int")]
        public const uint Unsigned = 1;

        [NativeTypeName("const int")]
        public const int ShiftSigned = 1 << Signed;

        [NativeTypeName("const int")]
        public const int ShiftSignedLong = 1 << SignedLong;

        [NativeTypeName("const int")]
        public const int ShiftUnsigned = 1 << (int)(Unsigned);

        [NativeTypeName("const int")]
        public const int Char = 1 << (sbyte)('a');

        [NativeTypeName("const int")]
        public const int Byte = 1 << (sbyte)(1);

        [NativeTypeName("const int")]
        public const int UByte = unchecked(1 << (byte)(1));

        [NativeTypeName("const int")]
        public const int CInt = 1 << 1;

        [NativeTypeName("const int")]
        public const int CUint = 1 << 1;

        [NativeTypeName("const int")]
        public const int Negative = 1 << -1;

        [NativeTypeName("const int")]
        public const int OutOfRangePos = unchecked(1 << (int)(10000000000));

        [NativeTypeName("const int")]
        public const int OutOfRangeNeg = unchecked(1 << (int)(-10000000000));

        [NativeTypeName("const int")]
        public const int IntMax = 1 << 2147483647;

        [NativeTypeName("const int")]
        public const int IntMin = unchecked(1 << -2147483648);

        [NativeTypeName("const int")]
        public const int LongMax = unchecked(1 << (int)(9223372036854775807));

        [NativeTypeName("const int")]
        public const int LongMin = unchecked(1 << (int)(-9223372036854775808));

        [NativeTypeName("const int")]
        public const int ULongMax = 1 << -1;

        [NativeTypeName("const int")]
        public const int Hexadecimal = 1 << 1;

        [NativeTypeName("#define Left 1 << 1U")]
        public const int Left = 1 << 1;

        [NativeTypeName("#define Right 1 >> 1U")]
        public const int Right = 1 >> 1;

        [NativeTypeName("#define Int 1 << 1")]
        public const int Int = 1 << 1;

        [NativeTypeName("#define Long 1 << 1L")]
        public const int Long = 1 << 1;

        [NativeTypeName("#define LongLong 1 << 1LL")]
        public const int LongLong = 1 << 1;

        [NativeTypeName("#define ULong 1 << 1UL")]
        public const int ULong = 1 << 1;

        [NativeTypeName("#define ULongLong 1 << 1ULL")]
        public const int ULongLong = 1 << 1;

        [NativeTypeName("#define Complex ((((unsigned int)(0)) << 29U) | (((unsigned int)(1)) << 22U) | (((unsigned int)(0)) << 12U) | ((unsigned int)(0)))")]
        public const uint Complex = ((((uint)(0)) << 29) | (((uint)(1)) << 22) | (((uint)(0)) << 12) | ((uint)(0)));
    }
}
