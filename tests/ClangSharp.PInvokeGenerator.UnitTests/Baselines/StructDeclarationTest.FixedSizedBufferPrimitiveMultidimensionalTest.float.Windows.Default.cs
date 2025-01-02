namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("float[2][1][3][4]")]
        public fixed float c[2 * 1 * 3 * 4];
    }
}
