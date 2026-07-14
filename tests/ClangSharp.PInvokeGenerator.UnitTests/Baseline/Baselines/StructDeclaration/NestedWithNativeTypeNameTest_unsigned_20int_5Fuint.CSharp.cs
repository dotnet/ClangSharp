namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("unsigned int")]
        public uint r;

        [NativeTypeName("unsigned int")]
        public uint g;

        [NativeTypeName("unsigned int")]
        public uint b;

        public partial struct MyNestedStruct
        {
            [NativeTypeName("unsigned int")]
            public uint r;

            [NativeTypeName("unsigned int")]
            public uint g;

            [NativeTypeName("unsigned int")]
            public uint b;

            [NativeTypeName("unsigned int")]
            public uint a;
        }
    }
}
