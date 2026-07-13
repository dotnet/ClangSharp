namespace ClangSharp.Test
{
    public unsafe partial struct example_s
    {
        [NativeTypeName("example_t *")]
        public example_s* next;

        public void* data;
    }
}
