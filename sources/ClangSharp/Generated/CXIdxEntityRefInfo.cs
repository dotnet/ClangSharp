namespace ClangSharp
{
    public unsafe partial struct CXIdxEntityRefInfo
    {
        public CXIdxEntityRefKind kind;

        public CXCursor cursor;

        public CXIdxLoc loc;

        public CXIdxEntityInfo* referencedEntity;

        public CXIdxEntityInfo* parentEntity;

        public CXIdxContainerInfo* container;

        public CXSymbolRole role;
    }
}
