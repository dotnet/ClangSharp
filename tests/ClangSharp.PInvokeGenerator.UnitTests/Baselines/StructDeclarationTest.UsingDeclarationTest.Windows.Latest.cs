namespace ClangSharp.Test
{
    public partial struct MyStruct1A
    {
        public void MyMethod()
        {
        }
    }

    [NativeTypeName("struct MyStruct1B : MyStruct1A")]
    public partial struct MyStruct1B
    {
        public void MyMethod()
        {
        }
    }
}
