using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class CXXNullPtrLiteralExpr : Expr
    {
        public CXXNullPtrLiteralExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXNullPtrLiteralExpr);
        }
    }
}
