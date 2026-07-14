namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public int r;

        [NativeTypeName("MyTypedefAlias")]
        public int g;

        [NativeTypeName("MyTypedefAlias")]
        public int b;
    }
}
