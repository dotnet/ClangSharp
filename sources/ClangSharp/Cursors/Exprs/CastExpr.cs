using ClangSharp.Interop;

namespace ClangSharp
{
    public class CastExpr : Expr
    {
        protected CastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
