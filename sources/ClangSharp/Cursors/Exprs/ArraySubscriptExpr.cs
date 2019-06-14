using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArraySubscriptExpr : Expr
    {
        public ArraySubscriptExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ArraySubscriptExpr);
        }
    }
}
