using System;

namespace ClangSharp
{
    public partial struct CXIdxObjCCategoryDeclInfo
    {
        public IntPtr containerInfo;
        public IntPtr objcClass;
        public CXCursor classCursor;
        public CXIdxLoc classLoc;
        public IntPtr protocols;
    }
}
