using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class ParenExpr : Expr
    {
        private Expr _subExpr;

        public ParenExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ParenExpr);
        }

        public Expr SubExpr => _subExpr;

        protected override Expr GetOrAddExpr(CXCursor childHandle)
        {
            var expr = base.GetOrAddExpr(childHandle);

            Debug.Assert(_subExpr is null);
            _subExpr = expr;

            return expr;
        }
    }
}
