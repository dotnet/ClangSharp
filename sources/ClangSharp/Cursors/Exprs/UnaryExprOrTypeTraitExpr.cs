using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryExprOrTypeTraitExpr : Expr
    {
        internal UnaryExprOrTypeTraitExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryExpr)
        {
        }
    }
}
