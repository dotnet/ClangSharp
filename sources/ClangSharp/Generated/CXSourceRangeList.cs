namespace ClangSharp
{
    public unsafe partial struct CXSourceRangeList
    {
        [NativeTypeName("unsigned int")]
        public uint count;

        [NativeTypeName("CXSourceRange *")]
        public CXSourceRange* ranges;
    }
}
