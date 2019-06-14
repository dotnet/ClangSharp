using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryExprOrTypeTraitExpr : Expr
    {
        public UnaryExprOrTypeTraitExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnaryExpr);
        }
    }
}
