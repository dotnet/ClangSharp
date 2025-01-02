namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTypedefAlias")]
        public ulong r;

        [NativeTypeName("MyTypedefAlias")]
        public ulong g;

        [NativeTypeName("MyTypedefAlias")]
        public ulong b;
    }
}
