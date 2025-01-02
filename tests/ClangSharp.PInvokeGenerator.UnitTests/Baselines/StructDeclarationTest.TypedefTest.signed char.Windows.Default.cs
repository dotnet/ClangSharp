namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public sbyte r;

        [NativeTypeName("MyTypedefAlias")]
        public sbyte g;

        [NativeTypeName("MyTypedefAlias")]
        public sbyte b;
    }
}
