namespace ClangSharp
{
    public unsafe partial struct CXSourceRangeList
    {
        public uint count;

        public CXSourceRange* ranges;
    }
}
