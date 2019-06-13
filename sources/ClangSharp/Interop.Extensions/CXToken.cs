namespace ClangSharp.Interop
{
    public unsafe partial struct CXToken
    {
        public CXTokenKind Kind => clang.getTokenKind(this);

        public CXSourceRange GetExtent(CXTranslationUnit translationUnit) => clang.getTokenExtent(translationUnit, this);

        public CXSourceLocation GetLocation(CXTranslationUnit translationUnit) => clang.getTokenLocation(translationUnit, this);

        public CXString GetSpelling(CXTranslationUnit translationUnit) => clang.getTokenSpelling(translationUnit, this);
    }
}
