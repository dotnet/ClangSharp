namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxIncludedFileInfo
    {
        public CXIdxLoc hashLoc;
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] public string filename;
        public CXFile file;
        public int isImport;
        public int isAngled;
        public int isModuleImport;
    }
}
