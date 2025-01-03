namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("MyBuffer")]
        public fixed short c[3];
    }
}
