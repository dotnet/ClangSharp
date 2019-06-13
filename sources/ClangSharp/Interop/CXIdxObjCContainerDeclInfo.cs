namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxObjCContainerDeclInfo
    {
        [NativeTypeName("const CXIdxDeclInfo *")]
        public CXIdxDeclInfo* declInfo;

        public CXIdxObjCContainerKind kind;
    }
}
