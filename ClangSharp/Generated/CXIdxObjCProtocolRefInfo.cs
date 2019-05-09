namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxObjCProtocolRefInfo
    {
        public IntPtr protocol;
        public CXCursor cursor;
        public CXIdxLoc loc;
    }
}
