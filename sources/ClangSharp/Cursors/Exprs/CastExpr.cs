using ClangSharp.Interop;

namespace ClangSharp
{
    public class CastExpr : Expr
    {
        private protected CastExpr(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
