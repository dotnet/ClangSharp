namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("signed char[2][1][3][4]")]
        public fixed sbyte c[2 * 1 * 3 * 4];
    }
}
