namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("unsigned char")]
        public byte r;

        [NativeTypeName("unsigned char")]
        public byte g;

        [NativeTypeName("unsigned char")]
        public byte b;
    }
}
