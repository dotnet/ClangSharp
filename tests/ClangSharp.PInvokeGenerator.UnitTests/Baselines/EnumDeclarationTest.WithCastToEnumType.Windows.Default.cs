namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0 = (int)(MyEnum)(10),
        MyEnum_Value1 = (int)(MyEnum)(MyEnum_Value0),
        MyEnum_Value2 = ((int)(MyEnum)(10)) + MyEnum_Value1,
    }
}
