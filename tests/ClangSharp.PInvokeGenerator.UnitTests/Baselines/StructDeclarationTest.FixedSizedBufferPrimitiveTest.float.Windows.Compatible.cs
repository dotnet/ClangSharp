namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("float[3]")]
        public fixed float c[3];
    }
}
