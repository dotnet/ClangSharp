namespace ClangSharp
{
    public partial struct CXTUResourceUsageEntry
    {
        public string Name => clang.getTUResourceUsageName(kind);
    }
}
