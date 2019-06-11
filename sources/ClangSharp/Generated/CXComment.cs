namespace ClangSharp
{
    public unsafe partial struct CXComment
    {
        [NativeTypeName("const void *")]
        public void* ASTNode;

        [NativeTypeName("CXTranslationUnit")]
        public CXTranslationUnitImpl* TranslationUnit;
    }
}
