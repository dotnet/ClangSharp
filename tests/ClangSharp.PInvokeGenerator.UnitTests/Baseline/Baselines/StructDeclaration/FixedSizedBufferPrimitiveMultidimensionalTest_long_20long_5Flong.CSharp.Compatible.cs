namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("long long[2][1][3][4]")]
        public fixed long c[2 * 1 * 3 * 4];
    }
}
