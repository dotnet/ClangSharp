namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned short[3]")]
        public fixed ushort c[3];
    }
}
