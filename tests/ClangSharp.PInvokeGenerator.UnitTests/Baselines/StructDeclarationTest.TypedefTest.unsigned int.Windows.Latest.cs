namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public uint r;

        [NativeTypeName("MyTypedefAlias")]
        public uint g;

        [NativeTypeName("MyTypedefAlias")]
        public uint b;
    }
}
