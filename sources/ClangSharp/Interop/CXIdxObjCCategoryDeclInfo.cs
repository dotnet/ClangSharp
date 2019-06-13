namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxObjCCategoryDeclInfo
    {
        [NativeTypeName("const CXIdxObjCContainerDeclInfo *")]
        public CXIdxObjCContainerDeclInfo* containerInfo;

        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* objcClass;

        public CXCursor classCursor;

        public CXIdxLoc classLoc;

        [NativeTypeName("const CXIdxObjCProtocolRefListInfo *")]
        public CXIdxObjCProtocolRefListInfo* protocols;
    }
}
