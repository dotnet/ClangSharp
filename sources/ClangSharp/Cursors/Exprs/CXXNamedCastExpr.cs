using ClangSharp.Interop;

namespace ClangSharp
{
    public class CXXNamedCastExpr : ExplicitCastExpr
    {
        protected CXXNamedCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
