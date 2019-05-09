namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxAttrInfo
    {
        public CXIdxAttrKind kind;
        public CXCursor cursor;
        public CXIdxLoc loc;
    }
}
