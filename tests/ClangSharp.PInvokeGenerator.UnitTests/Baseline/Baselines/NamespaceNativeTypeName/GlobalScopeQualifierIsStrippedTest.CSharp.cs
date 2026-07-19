namespace ClangSharp.Test
{
    public partial struct @boolean
    {
        public int x;
    }

    public unsafe partial struct Holder
    {
        [NativeTypeName("boolean *")]
        public byte* globalPtr;
    }
}
