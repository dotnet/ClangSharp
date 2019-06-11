namespace ClangSharp
{
    public unsafe partial struct CXIdxIncludedFileInfo
    {
        public CXIdxLoc hashLoc;

        [NativeTypeName("const char *")]
        public sbyte* filename;

        [NativeTypeName("CXFile")]
        public void* file;

        public int isImport;

        public int isAngled;

        public int isModuleImport;
    }
}
