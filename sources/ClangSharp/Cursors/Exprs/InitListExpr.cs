using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class InitListExpr : Expr
    {
        internal InitListExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_InitListExpr)
        {
        }
    }
}
