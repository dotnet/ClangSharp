using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXThisExpr : Expr
    {
        internal CXXThisExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXThisExpr)
        {
        }
    }
}
