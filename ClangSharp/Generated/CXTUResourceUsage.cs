using System;

namespace ClangSharp
{
    public partial struct CXTUResourceUsage
    {
        public IntPtr data;
        public uint numEntries;
        public IntPtr entries;
    }
}
