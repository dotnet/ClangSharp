using static ClangSharp.Test.MyEnum1;

namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value0 = 10,
    }

    public enum MyEnum2
    {
        MyEnum2_Value0 = MyEnum1_Value0,
        MyEnum2_Value1 = MyEnum1_Value0 + (int)(MyEnum1)(10),
    }
}
