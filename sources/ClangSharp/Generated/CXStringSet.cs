namespace ClangSharp
{
    public unsafe partial struct CXStringSet
    {
        [NativeTypeName("CXString *")]
        public CXString* Strings;

        [NativeTypeName("unsigned int")]
        public uint Count;
    }
}
