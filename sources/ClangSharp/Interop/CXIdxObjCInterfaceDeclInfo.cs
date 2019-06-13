namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxObjCInterfaceDeclInfo
    {
        [NativeTypeName("const CXIdxObjCContainerDeclInfo *")]
        public CXIdxObjCContainerDeclInfo* containerInfo;

        [NativeTypeName("const CXIdxBaseClassInfo *")]
        public CXIdxBaseClassInfo* superInfo;

        [NativeTypeName("const CXIdxObjCProtocolRefListInfo *")]
        public CXIdxObjCProtocolRefListInfo* protocols;
    }
}
