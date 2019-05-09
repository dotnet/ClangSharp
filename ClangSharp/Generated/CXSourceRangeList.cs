namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXSourceRangeList
    {
        public uint count;
        public IntPtr ranges;
    }
}
