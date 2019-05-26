using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnaryExpr : Expr
    {
        public UnaryExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnaryExpr);
        }
    }
}
