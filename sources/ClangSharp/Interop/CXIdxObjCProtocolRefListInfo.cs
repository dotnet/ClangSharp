namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxObjCProtocolRefListInfo
    {
        [NativeTypeName("const CXIdxObjCProtocolRefInfo *const *")]
        public CXIdxObjCProtocolRefInfo** protocols;

        [NativeTypeName("unsigned int")]
        public uint numProtocols;
    }
}
