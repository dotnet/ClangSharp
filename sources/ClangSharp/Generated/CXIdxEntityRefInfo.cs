namespace ClangSharp
{
    public unsafe partial struct CXIdxEntityRefInfo
    {
        public CXIdxEntityRefKind kind;

        public CXCursor cursor;

        public CXIdxLoc loc;

        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* referencedEntity;

        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* parentEntity;

        [NativeTypeName("const CXIdxContainerInfo *")]
        public CXIdxContainerInfo* container;

        public CXSymbolRole role;
    }
}
