using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PackExpansionExpr : Expr
    {
        internal PackExpansionExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_PackExpansionExpr)
        {
        }
    }
}
