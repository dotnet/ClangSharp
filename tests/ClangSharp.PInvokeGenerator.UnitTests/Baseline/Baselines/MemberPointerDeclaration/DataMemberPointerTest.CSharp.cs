namespace ClangSharp.Test
{
    public partial struct MyClass
    {
    }

    public unsafe partial struct MyStruct
    {
        [NativeTypeName("int MyClass::*")]
        public void* field;
    }
}
