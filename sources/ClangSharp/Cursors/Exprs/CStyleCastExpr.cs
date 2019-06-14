using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CStyleCastExpr : ExplicitCastExpr
    {
        public CStyleCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CStyleCastExpr);
        }
    }
}
