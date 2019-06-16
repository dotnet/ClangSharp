using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXNullPtrLiteralExpr : Expr
    {
        internal CXXNullPtrLiteralExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXNullPtrLiteralExpr)
        {
        }
    }
}
