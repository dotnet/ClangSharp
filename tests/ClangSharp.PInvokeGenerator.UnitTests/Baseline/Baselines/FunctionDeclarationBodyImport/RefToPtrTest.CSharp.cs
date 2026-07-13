namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;
    }

    public static unsafe partial class Methods
    {
        public static bool MyFunction([NativeTypeName("const MyStruct &")] MyStruct* lhs, [NativeTypeName("const MyStruct &")] MyStruct* rhs)
        {
            return lhs->value == rhs->value;
        }
    }
}
