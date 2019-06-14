namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxBaseClassInfo
    {
        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* @base;

        public CXCursor cursor;

        public CXIdxLoc loc;
    }
}
