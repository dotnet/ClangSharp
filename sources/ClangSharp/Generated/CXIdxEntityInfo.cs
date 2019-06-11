namespace ClangSharp
{
    public unsafe partial struct CXIdxEntityInfo
    {
        public CXIdxEntityKind kind;

        public CXIdxEntityCXXTemplateKind templateKind;

        public CXIdxEntityLanguage lang;

        [NativeTypeName("const char *")]
        public sbyte* name;

        [NativeTypeName("const char *")]
        public sbyte* USR;

        public CXCursor cursor;

        [NativeTypeName("const CXIdxAttrInfo *const *")]
        public CXIdxAttrInfo** attributes;

        [NativeTypeName("unsigned int")]
        public uint numAttributes;
    }
}
