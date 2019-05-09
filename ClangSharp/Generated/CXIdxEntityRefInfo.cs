namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

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
