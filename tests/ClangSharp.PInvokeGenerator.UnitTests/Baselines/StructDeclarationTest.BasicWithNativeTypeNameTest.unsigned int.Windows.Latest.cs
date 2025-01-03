namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("unsigned int")]
        public uint r;

        [NativeTypeName("unsigned int")]
        public uint g;

        [NativeTypeName("unsigned int")]
        public uint b;
    }
}
