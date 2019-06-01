using System.Runtime.InteropServices;

namespace ClangSharp
{
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
