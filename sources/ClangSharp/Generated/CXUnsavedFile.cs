namespace ClangSharp
{
    public unsafe partial struct CXUnsavedFile
    {
        [NativeTypeName("const sbyte*")]
        public sbyte* Filename;

        [NativeTypeName("const sbyte*")]
        public sbyte* Contents;

        public uint Length;
    }
}
