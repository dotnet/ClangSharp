namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("long long[3]")]
        public fixed long c[3];
    }
}
