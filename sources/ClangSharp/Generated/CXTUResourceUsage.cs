namespace ClangSharp
{
    public unsafe partial struct CXTUResourceUsage
    {
        public void* data;

        public uint numEntries;

        public CXTUResourceUsageEntry* entries;
    }
}
