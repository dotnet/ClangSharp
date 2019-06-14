using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnexposedExpr : Expr
    {
        public UnexposedExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnexposedExpr);
        }
    }
}
