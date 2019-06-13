using ClangSharp.Interop;

namespace ClangSharp
{
    internal class CXXNamedCastExpr : ExplicitCastExpr
    {
        protected CXXNamedCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
