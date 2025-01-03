namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("int[]")]
        public fixed int x[1];
    }
}
