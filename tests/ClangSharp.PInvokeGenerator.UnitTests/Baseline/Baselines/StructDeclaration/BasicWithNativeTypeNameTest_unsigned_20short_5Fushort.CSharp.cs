namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("unsigned short")]
        public ushort r;

        [NativeTypeName("unsigned short")]
        public ushort g;

        [NativeTypeName("unsigned short")]
        public ushort b;
    }
}
