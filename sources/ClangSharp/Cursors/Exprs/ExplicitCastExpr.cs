using ClangSharp.Interop;

namespace ClangSharp
{
    public class ExplicitCastExpr : CastExpr
    {
        private protected ExplicitCastExpr(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
