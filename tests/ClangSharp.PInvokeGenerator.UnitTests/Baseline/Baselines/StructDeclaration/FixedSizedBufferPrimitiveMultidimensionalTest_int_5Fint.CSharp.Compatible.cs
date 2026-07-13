namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("int[2][1][3][4]")]
        public fixed int c[2 * 1 * 3 * 4];
    }
}
