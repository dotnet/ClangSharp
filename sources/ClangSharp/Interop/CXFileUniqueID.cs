namespace ClangSharp.Interop
{
    public unsafe partial struct CXFileUniqueID
    {
        [NativeTypeName("unsigned long long [3]")]
        public fixed ulong data[3];
    }
}
