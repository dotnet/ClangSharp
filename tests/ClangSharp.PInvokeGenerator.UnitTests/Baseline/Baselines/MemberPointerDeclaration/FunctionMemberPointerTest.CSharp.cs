namespace ClangSharp.Test
{
    public partial struct MyClass
    {
    }

    public unsafe partial struct MyStruct
    {
        [NativeTypeName("void (MyClass::*)(int)")]
        public void* callback;
    }
}
