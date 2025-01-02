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

        public partial struct MyNestedStruct
        {
            [NativeTypeName("unsigned char")]
            public byte r;

            [NativeTypeName("unsigned char")]
            public byte g;

            [NativeTypeName("unsigned char")]
            public byte b;

            [NativeTypeName("unsigned char")]
            public byte a;
        }
    }
}
