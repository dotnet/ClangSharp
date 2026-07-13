namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("const long")]
        public const nint SignedLong = 1;

        [NativeTypeName("const int")]
        public const int ShiftSignedLong = 1 << (int)(SignedLong);
    }
}
