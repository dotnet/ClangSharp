namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXString
    {
        public IntPtr data;
        public uint private_flags;
    }
}
