namespace ClangSharp
{
    public unsafe partial struct CXIdxObjCProtocolRefInfo
    {
        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* protocol;

        public CXCursor cursor;

        public CXIdxLoc loc;
    }
}
