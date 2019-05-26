using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CStyleCastExpr : Expr
    {
        public CStyleCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CStyleCastExpr);
        }
    }
}
