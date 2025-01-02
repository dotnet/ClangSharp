namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("short[]")]
        public fixed short x[1];
    }
}
