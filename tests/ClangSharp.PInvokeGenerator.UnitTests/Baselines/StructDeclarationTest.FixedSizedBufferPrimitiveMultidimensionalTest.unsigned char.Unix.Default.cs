namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned char[2][1][3][4]")]
        public fixed byte c[2 * 1 * 3 * 4];
    }
}
