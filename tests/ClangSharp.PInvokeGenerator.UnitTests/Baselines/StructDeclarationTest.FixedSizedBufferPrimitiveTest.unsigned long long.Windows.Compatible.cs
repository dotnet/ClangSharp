namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("unsigned long long[3]")]
        public fixed ulong c[3];
    }
}
