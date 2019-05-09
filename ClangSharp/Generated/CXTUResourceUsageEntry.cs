namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXTUResourceUsageEntry
    {
        public CXTUResourceUsageKind kind;
        public uint amount;
    }
}
