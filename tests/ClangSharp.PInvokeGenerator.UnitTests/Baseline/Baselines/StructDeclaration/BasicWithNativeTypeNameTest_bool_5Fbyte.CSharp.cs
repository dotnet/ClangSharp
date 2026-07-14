namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("bool")]
        public byte r;

        [NativeTypeName("bool")]
        public byte g;

        [NativeTypeName("bool")]
        public byte b;
    }
}
