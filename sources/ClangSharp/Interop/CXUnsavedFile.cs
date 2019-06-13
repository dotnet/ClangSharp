namespace ClangSharp.Interop
{
    public unsafe partial struct CXUnsavedFile
    {
        [NativeTypeName("const char *")]
        public sbyte* Filename;

        [NativeTypeName("const char *")]
        public sbyte* Contents;

        [NativeTypeName("unsigned long")]
        public uint Length;
    }
}
