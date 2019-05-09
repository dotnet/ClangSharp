namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxDeclInfo
    {
        public IntPtr entityInfo;
        public CXCursor cursor;
        public CXIdxLoc loc;
        public IntPtr semanticContainer;
        public IntPtr lexicalContainer;
        public int isRedeclaration;
        public int isDefinition;
        public int isContainer;
        public IntPtr declAsContainer;
        public int isImplicit;
        public IntPtr attributes;
        public uint numAttributes;
        public uint flags;
    }
}
