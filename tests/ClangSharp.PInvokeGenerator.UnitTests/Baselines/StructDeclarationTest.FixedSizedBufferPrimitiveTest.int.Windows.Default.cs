namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("int[3]")]
        public fixed int c[3];
    }
}
