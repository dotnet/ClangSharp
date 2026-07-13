using static ClangSharp.Test.MyEnum1;

namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value1 = 1,
    }

    public enum MyEnum2
    {
        MyEnum2_Value1 = MyEnum1_Value1,
    }
}
