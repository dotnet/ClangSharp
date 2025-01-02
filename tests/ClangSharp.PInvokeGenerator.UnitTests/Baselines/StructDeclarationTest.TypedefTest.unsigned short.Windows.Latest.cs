namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public ushort r;

        [NativeTypeName("MyTypedefAlias")]
        public ushort g;

        [NativeTypeName("MyTypedefAlias")]
        public ushort b;
    }
}
