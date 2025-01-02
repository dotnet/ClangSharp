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

        public partial struct MyNestedStruct
        {
            [NativeTypeName("bool")]
            public byte r;

            [NativeTypeName("bool")]
            public byte g;

            [NativeTypeName("bool")]
            public byte b;

            [NativeTypeName("bool")]
            public byte a;
        }
    }
}
