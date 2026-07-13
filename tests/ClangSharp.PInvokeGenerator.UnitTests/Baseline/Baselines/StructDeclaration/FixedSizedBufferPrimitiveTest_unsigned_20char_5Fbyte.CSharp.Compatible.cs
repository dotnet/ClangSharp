namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned char[3]")]
        public fixed byte c[3];
    }
}
