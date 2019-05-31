using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXConstCastExpr : CXXNamedCastExpr
    {
        public CXXConstCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXConstCastExpr);
        }
    }
}
