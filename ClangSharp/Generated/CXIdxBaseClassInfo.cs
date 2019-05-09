namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxBaseClassInfo
    {
        public IntPtr @base;
        public CXCursor cursor;
        public CXIdxLoc loc;
    }
}
