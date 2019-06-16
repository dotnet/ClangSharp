using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXStaticCastExpr : CXXNamedCastExpr
    {
        internal CXXStaticCastExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXStaticCastExpr)
        {
        }
    }
}
