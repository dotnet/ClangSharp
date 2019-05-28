using System.Diagnostics;

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

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsExpression)
            {
                var expr = GetOrAddChild<Expr>(childHandle);

                Debug.Assert(_subExpr is null);
                _subExpr = expr;

                return expr.Visit(clientData);
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
