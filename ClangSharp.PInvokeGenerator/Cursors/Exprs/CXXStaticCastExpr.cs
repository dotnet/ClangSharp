using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXStaticCastExpr : Expr
    {
        public CXXStaticCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXStaticCastExpr);
        }
    }
}
