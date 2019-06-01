using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXFunctionalCastExpr : ExplicitCastExpr
    {
        public CXXFunctionalCastExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXFunctionalCastExpr);
        }
    }
}
