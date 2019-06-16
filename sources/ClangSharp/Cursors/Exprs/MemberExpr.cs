using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MemberExpr : Expr
    {
        internal MemberExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MemberRefExpr)
        {
        }
    }
}
