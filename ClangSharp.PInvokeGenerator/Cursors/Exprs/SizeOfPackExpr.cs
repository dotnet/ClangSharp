using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class SizeOfPackExpr : Expr
    {
        public SizeOfPackExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_SizeOfPackExpr);
        }
    }
}
