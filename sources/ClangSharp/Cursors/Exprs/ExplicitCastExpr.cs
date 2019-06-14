using ClangSharp.Interop;

namespace ClangSharp
{
    public class ExplicitCastExpr : CastExpr
    {
        protected ExplicitCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
