using ClangSharp.Interop;

namespace ClangSharp
{
    internal class ExplicitCastExpr : CastExpr
    {
        protected ExplicitCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
