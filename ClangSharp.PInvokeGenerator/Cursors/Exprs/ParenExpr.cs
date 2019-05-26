using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ParenExpr : Expr
    {
        public ParenExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ParenExpr);
        }
    }
}
