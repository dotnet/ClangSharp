namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value0,
    }

    [NativeTypeName("int")]
    public enum MyEnum2 : uint
    {
        MyEnum2_Value0,
    }
}
