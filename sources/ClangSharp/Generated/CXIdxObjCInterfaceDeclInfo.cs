namespace ClangSharp
{
    public unsafe partial struct CXIdxObjCInterfaceDeclInfo
    {
        public CXIdxObjCContainerDeclInfo* containerInfo;

        public CXIdxBaseClassInfo* superInfo;

        public CXIdxObjCProtocolRefListInfo* protocols;
    }
}
