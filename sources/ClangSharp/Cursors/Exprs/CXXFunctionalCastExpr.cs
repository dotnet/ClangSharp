using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXFunctionalCastExpr : ExplicitCastExpr
    {
        internal CXXFunctionalCastExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXFunctionalCastExpr)
        {
        }
    }
}
