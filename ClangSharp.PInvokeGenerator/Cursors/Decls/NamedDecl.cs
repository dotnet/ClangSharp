namespace ClangSharp
{
    internal class NamedDecl : Decl
    {
        protected NamedDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public CXLinkageKind Linkage => Handle.Linkage;

        public CXVisibilityKind Visibility => Handle.Visibility;
    }
}
