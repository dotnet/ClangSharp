using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXStaticCastExpr : CXXNamedCastExpr
    {
        public CXXStaticCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXStaticCastExpr);
        }
    }
}
