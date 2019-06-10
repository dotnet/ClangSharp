namespace ClangSharp
{
    public unsafe partial struct CXIdxEntityInfo
    {
        public CXIdxEntityKind kind;

        public CXIdxEntityCXXTemplateKind templateKind;

        public CXIdxEntityLanguage lang;

        [NativeTypeName("const sbyte*")]
        public sbyte* name;

        [NativeTypeName("const sbyte*")]
        public sbyte* USR;

        public CXCursor cursor;

        [NativeTypeName("const CXIdxAttrInfo*const*")]
        public CXIdxAttrInfo** attributes;

        public uint numAttributes;
    }
}
