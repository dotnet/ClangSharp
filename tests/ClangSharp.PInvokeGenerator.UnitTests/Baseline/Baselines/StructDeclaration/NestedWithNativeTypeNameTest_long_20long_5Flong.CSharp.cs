namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("long long")]
        public long r;

        [NativeTypeName("long long")]
        public long g;

        [NativeTypeName("long long")]
        public long b;

        public partial struct MyNestedStruct
        {
            [NativeTypeName("long long")]
            public long r;

            [NativeTypeName("long long")]
            public long g;

            [NativeTypeName("long long")]
            public long b;

            [NativeTypeName("long long")]
            public long a;
        }
    }
}
