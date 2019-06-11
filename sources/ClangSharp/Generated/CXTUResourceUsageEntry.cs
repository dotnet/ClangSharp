namespace ClangSharp
{
    public partial struct CXTUResourceUsageEntry
    {
        [NativeTypeName("enum CXTUResourceUsageKind")]
        public CXTUResourceUsageKind kind;

        [NativeTypeName("unsigned long")]
        public uint amount;
    }
}
