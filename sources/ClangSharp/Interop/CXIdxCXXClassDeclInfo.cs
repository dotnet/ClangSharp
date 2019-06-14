namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxCXXClassDeclInfo
    {
        [NativeTypeName("const CXIdxDeclInfo *")]
        public CXIdxDeclInfo* declInfo;

        [NativeTypeName("const CXIdxBaseClassInfo *const *")]
        public CXIdxBaseClassInfo** bases;

        [NativeTypeName("unsigned int")]
        public uint numBases;
    }
}
