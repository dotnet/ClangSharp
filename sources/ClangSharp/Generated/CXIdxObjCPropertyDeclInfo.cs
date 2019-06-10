namespace ClangSharp
{
    public unsafe partial struct CXIdxObjCPropertyDeclInfo
    {
        public CXIdxDeclInfo* declInfo;

        public CXIdxEntityInfo* getter;

        public CXIdxEntityInfo* setter;
    }
}
