namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned short[2][1][3][4]")]
        public fixed ushort c[2 * 1 * 3 * 4];
    }
}
