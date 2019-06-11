namespace ClangSharp
{
    public unsafe partial struct CXIdxDeclInfo
    {
        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* entityInfo;

        public CXCursor cursor;

        public CXIdxLoc loc;

        [NativeTypeName("const CXIdxContainerInfo *")]
        public CXIdxContainerInfo* semanticContainer;

        [NativeTypeName("const CXIdxContainerInfo *")]
        public CXIdxContainerInfo* lexicalContainer;

        public int isRedeclaration;

        public int isDefinition;

        public int isContainer;

        [NativeTypeName("const CXIdxContainerInfo *")]
        public CXIdxContainerInfo* declAsContainer;

        public int isImplicit;

        [NativeTypeName("const CXIdxAttrInfo *const *")]
        public CXIdxAttrInfo** attributes;

        [NativeTypeName("unsigned int")]
        public uint numAttributes;

        [NativeTypeName("unsigned int")]
        public uint flags;
    }
}
