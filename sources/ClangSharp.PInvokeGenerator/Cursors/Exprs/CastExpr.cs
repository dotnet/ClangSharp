using ClangSharp.Interop;

namespace ClangSharp
{
    internal class CastExpr : Expr
    {
        protected CastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
