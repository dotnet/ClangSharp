namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXTUResourceUsage
    {
        public IntPtr data;
        public uint numEntries;
        public IntPtr entries;
    }
}
