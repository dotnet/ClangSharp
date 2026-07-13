namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned int[2][1][3][4]")]
        public fixed uint c[2 * 1 * 3 * 4];
    }
}
