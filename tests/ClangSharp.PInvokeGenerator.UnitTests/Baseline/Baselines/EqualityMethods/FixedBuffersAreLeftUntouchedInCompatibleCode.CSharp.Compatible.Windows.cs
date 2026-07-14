namespace ClangSharp.Test
{
    public unsafe partial struct WithArray
    {
        [NativeTypeName("int[4]")]
        public fixed int Values[4];
    }
}
