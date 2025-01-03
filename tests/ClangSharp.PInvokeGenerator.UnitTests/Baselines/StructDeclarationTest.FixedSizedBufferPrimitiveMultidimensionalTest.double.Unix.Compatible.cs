namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("double[2][1][3][4]")]
        public fixed double c[2 * 1 * 3 * 4];
    }
}
