using ClangSharp.Interop;

namespace ClangSharp
{
    public class CXXNamedCastExpr : ExplicitCastExpr
    {
        private protected CXXNamedCastExpr(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
