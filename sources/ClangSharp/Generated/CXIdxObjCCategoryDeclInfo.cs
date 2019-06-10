namespace ClangSharp
{
    public unsafe partial struct CXIdxObjCCategoryDeclInfo
    {
        public CXIdxObjCContainerDeclInfo* containerInfo;

        public CXIdxEntityInfo* objcClass;

        public CXCursor classCursor;

        public CXIdxLoc classLoc;

        public CXIdxObjCProtocolRefListInfo* protocols;
    }
}
