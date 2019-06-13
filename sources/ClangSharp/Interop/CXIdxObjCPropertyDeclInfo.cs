namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxObjCPropertyDeclInfo
    {
        [NativeTypeName("const CXIdxDeclInfo *")]
        public CXIdxDeclInfo* declInfo;

        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* getter;

        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* setter;
    }
}
