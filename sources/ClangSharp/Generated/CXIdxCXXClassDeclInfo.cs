namespace ClangSharp
{
    public unsafe partial struct CXIdxCXXClassDeclInfo
    {
        public CXIdxDeclInfo* declInfo;

        [NativeTypeName("const CXIdxBaseClassInfo*const*")]
        public CXIdxBaseClassInfo** bases;

        public uint numBases;
    }
}
