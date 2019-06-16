using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CStyleCastExpr : ExplicitCastExpr
    {
        internal CStyleCastExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CStyleCastExpr)
        {
        }
    }
}
