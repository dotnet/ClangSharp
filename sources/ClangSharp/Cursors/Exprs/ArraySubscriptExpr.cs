using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArraySubscriptExpr : Expr
    {
        internal ArraySubscriptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ArraySubscriptExpr)
        {
        }
    }
}
