namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("short[3]")]
        public fixed short c[3];
    }
}
