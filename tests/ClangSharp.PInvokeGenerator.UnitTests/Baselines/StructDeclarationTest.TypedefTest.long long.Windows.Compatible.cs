namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public long r;

        [NativeTypeName("MyTypedefAlias")]
        public long g;

        [NativeTypeName("MyTypedefAlias")]
        public long b;
    }
}
