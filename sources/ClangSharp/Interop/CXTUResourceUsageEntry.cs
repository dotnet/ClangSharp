namespace ClangSharp.Interop
{
    public partial struct CXTUResourceUsageEntry
    {
        [NativeTypeName("enum CXTUResourceUsageKind")]
        public CXTUResourceUsageKind kind;

        [NativeTypeName("unsigned long")]
        public uint amount;
    }
}
