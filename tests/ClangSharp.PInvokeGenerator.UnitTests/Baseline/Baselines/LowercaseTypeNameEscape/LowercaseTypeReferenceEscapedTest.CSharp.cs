namespace ClangSharp.Test
{
    internal partial struct @config
    {
        public int value;
    }

    public unsafe partial struct @holder
    {
        [NativeTypeName("struct config *")]
        public @config* cfg;
    }
}
