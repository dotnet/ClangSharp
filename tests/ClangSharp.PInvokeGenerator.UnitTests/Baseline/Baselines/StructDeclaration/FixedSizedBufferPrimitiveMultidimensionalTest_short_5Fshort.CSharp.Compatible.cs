namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("short[2][1][3][4]")]
        public fixed short c[2 * 1 * 3 * 4];
    }
}
