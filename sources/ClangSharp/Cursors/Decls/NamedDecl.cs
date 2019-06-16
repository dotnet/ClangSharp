using ClangSharp.Interop;

namespace ClangSharp
{
    public class NamedDecl : Decl
    {
        private protected NamedDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        public CXLinkageKind LinkageInternal => Handle.Linkage;

        public string Name => Spelling;

        public CXVisibilityKind Visibility => Handle.Visibility;
    }
}
