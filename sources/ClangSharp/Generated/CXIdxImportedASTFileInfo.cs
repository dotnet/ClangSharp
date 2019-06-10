namespace ClangSharp
{
    public unsafe partial struct CXIdxImportedASTFileInfo
    {
        [NativeTypeName("CXFile")]
        public void* file;

        [NativeTypeName("CXModule")]
        public void* module;

        public CXIdxLoc loc;

        public int isImplicit;
    }
}
