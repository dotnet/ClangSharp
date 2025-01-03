namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("signed char[3]")]
        public fixed sbyte c[3];
    }
}
