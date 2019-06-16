using ClangSharp.Interop;

namespace ClangSharp
{
    public class DeclaratorDecl : ValueDecl
    {
        private protected DeclaratorDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
