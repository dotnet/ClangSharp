namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("MyBuffer")]
        public fixed int c[3];
    }
}
