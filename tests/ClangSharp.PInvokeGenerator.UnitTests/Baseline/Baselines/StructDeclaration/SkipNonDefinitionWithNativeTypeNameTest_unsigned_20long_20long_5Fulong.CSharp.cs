namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("unsigned long long")]
        public ulong r;

        [NativeTypeName("unsigned long long")]
        public ulong g;

        [NativeTypeName("unsigned long long")]
        public ulong b;
    }
}
