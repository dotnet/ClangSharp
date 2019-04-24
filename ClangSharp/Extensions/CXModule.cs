namespace ClangSharp
{
    public partial struct CXModule
    {
        public CXFile AstFile => clang.Module_getASTFile(this);

        public CXString FullName => clang.Module_getFullName(this);

        public bool IsSystem => clang.Module_isSystem(this) != 0;

        public CXString Name => clang.Module_getName(this);

        public CXModule Parent => clang.Module_getParent(this);

        public uint GetNumTopLevelHeaders(CXTranslationUnit translationUnit) => clang.Module_getNumTopLevelHeaders(translationUnit, this);

        public CXFile GetTopLevelHeader(CXTranslationUnit translationUnit, uint index) => clang.Module_getTopLevelHeader(translationUnit, this, index);

        public override string ToString() => FullName.ToString();
    }
}
