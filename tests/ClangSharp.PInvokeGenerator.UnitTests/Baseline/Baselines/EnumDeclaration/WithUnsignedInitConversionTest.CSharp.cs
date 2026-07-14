namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1 = ((int)(1U << 22)),
        MyEnum_Value2 = ((int)((1U << 22) | (1 << 12))),
        MyEnum_Value3 = unchecked((int)(0x80000000)),
    }
}
