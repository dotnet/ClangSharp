namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("MyBuffer")]
        public fixed sbyte c[3];
    }
}
