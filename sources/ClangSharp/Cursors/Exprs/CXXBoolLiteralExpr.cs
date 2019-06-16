using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXBoolLiteralExpr : Expr
    {
        internal CXXBoolLiteralExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXBoolLiteralExpr)
        {
        }
    }
}
