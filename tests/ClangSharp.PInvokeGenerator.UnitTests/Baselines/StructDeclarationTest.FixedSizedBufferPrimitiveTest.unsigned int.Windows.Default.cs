namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned int[3]")]
        public fixed uint c[3];
    }
}
