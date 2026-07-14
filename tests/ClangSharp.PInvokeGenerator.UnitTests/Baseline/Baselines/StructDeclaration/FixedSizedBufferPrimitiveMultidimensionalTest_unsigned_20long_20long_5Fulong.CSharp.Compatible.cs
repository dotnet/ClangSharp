namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned long long[2][1][3][4]")]
        public fixed ulong c[2 * 1 * 3 * 4];
    }
}
