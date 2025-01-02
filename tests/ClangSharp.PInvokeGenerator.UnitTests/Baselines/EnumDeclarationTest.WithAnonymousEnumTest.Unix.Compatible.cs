using static ClangSharp.Test.Methods;

namespace ClangSharp.Test
{
    public enum MyEnum2
    {
        MyEnum2_Value1 = MyEnum1_Value1,
    }

    public static partial class Methods
    {
        public const uint MyEnum1_Value1 = 1;
    }
}
