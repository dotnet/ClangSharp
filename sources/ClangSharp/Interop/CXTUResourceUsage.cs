namespace ClangSharp.Interop
{
    public unsafe partial struct CXTUResourceUsage
    {
        [NativeTypeName("void *")]
        public void* data;

        [NativeTypeName("unsigned int")]
        public uint numEntries;

        [NativeTypeName("CXTUResourceUsageEntry *")]
        public CXTUResourceUsageEntry* entries;
    }
}
