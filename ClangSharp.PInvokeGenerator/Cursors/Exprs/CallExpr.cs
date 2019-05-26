using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CallExpr : Expr
    {
        public CallExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CallExpr);
        }
    }
}
