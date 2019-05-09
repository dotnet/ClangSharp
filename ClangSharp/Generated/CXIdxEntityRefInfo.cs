using System;

namespace ClangSharp
{
    public partial struct CXIdxEntityRefInfo
    {
        public CXIdxEntityRefKind kind;
        public CXCursor cursor;
        public CXIdxLoc loc;
        public IntPtr referencedEntity;
        public IntPtr parentEntity;
        public IntPtr container;
        public CXSymbolRole role;
    }
}
