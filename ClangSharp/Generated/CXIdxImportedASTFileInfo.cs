namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxImportedASTFileInfo
    {
        public CXFile file;
        public CXModule module;
        public CXIdxLoc loc;
        public int isImplicit;
    }
}
