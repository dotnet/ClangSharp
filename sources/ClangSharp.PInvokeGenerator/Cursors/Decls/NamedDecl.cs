using ClangSharp.Interop;

namespace ClangSharp
{
    internal class NamedDecl : Decl
    {
        protected NamedDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public CXLinkageKind Linkage => Handle.Linkage;

        public string Name => Spelling;

        public CXVisibilityKind Visibility => Handle.Visibility;
    }
}
