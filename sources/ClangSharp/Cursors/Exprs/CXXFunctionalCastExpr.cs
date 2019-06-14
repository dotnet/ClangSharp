using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXFunctionalCastExpr : ExplicitCastExpr
    {
        public CXXFunctionalCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXFunctionalCastExpr);
        }
    }
}
