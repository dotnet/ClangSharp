using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class InitListExpr : Expr
    {
        public InitListExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_InitListExpr);
        }
    }
}
