namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public short r;

        [NativeTypeName("MyTypedefAlias")]
        public short g;

        [NativeTypeName("MyTypedefAlias")]
        public short b;
    }
}
