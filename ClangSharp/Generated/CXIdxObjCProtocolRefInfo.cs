using System;

namespace ClangSharp
{
    public partial struct CXIdxObjCProtocolRefInfo
    {
        public IntPtr protocol;
        public CXCursor cursor;
        public CXIdxLoc loc;
    }
}
