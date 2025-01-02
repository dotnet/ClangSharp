namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public byte r;

        [NativeTypeName("MyTypedefAlias")]
        public byte g;

        [NativeTypeName("MyTypedefAlias")]
        public byte b;
    }
}
