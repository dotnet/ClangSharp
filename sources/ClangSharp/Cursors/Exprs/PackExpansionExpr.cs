using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PackExpansionExpr : Expr
    {
        public PackExpansionExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_PackExpansionExpr);
        }
    }
}
