namespace ClangSharp
{
    public unsafe partial struct CXString
    {
        [NativeTypeName("const void*")]
        public void* data;

        public uint private_flags;
    }
}
