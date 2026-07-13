namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public const uint MyEnum1_Value1 = 1;

        [NativeTypeName("const int")]
        public const int MyEnum2_Value1 = (int)(MyEnum1_Value1) + 1;
    }
}
