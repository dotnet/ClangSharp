namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxCXXClassDeclInfo
    {
        public IntPtr declInfo;
        public IntPtr bases;
        public uint numBases;
    }
}
