namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public float r;

        [NativeTypeName("MyTypedefAlias")]
        public float g;

        [NativeTypeName("MyTypedefAlias")]
        public float b;
    }
}
