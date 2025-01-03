namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public double r;

        [NativeTypeName("MyTypedefAlias")]
        public double g;

        [NativeTypeName("MyTypedefAlias")]
        public double b;
    }
}
