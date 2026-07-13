namespace ClangSharp.Test
{
    [NativeTypeName("int")]
    public enum MyEnum : uint
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2 = 0x80000000,
    }
}
