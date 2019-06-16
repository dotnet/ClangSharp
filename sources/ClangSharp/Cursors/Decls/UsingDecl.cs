using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UsingDecl : NamedDecl, IMergeable<UsingDecl>
    {
        internal UsingDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UsingDeclaration)
        {
        }
    }
}
