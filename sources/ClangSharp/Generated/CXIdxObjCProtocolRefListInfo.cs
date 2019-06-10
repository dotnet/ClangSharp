namespace ClangSharp
{
    public unsafe partial struct CXIdxObjCProtocolRefListInfo
    {
        [NativeTypeName("const CXIdxObjCProtocolRefInfo*const*")]
        public CXIdxObjCProtocolRefInfo** protocols;

        public uint numProtocols;
    }
}
