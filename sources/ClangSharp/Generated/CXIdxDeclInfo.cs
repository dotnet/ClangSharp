namespace ClangSharp
{
    public unsafe partial struct CXIdxDeclInfo
    {
        public CXIdxEntityInfo* entityInfo;

        public CXCursor cursor;

        public CXIdxLoc loc;

        public CXIdxContainerInfo* semanticContainer;

        public CXIdxContainerInfo* lexicalContainer;

        public int isRedeclaration;

        public int isDefinition;

        public int isContainer;

        public CXIdxContainerInfo* declAsContainer;

        public int isImplicit;

        [NativeTypeName("const CXIdxAttrInfo*const*")]
        public CXIdxAttrInfo** attributes;

        public uint numAttributes;

        public uint flags;
    }
}
