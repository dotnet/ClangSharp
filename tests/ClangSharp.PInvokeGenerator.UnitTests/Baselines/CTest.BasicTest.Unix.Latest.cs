namespace ClangSharp.Test
{
    [NativeTypeName("unsigned int")]
    public enum MyEnum : uint
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public partial struct MyStruct
    {
        [NativeTypeName("enum_t")]
        public MyEnum _field;
    }
}
