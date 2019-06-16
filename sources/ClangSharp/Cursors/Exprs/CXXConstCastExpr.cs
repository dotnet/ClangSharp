using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXConstCastExpr : CXXNamedCastExpr
    {
        internal CXXConstCastExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXConstCastExpr)
        {
        }
    }
}
