namespace ClangSharp.Interop
{
    public unsafe partial struct CXString
    {
        [NativeTypeName("const void *")]
        public void* data;

        [NativeTypeName("unsigned int")]
        public uint private_flags;
    }
}
