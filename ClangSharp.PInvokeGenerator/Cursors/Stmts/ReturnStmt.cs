using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ReturnStmt : Stmt
    {
        private Expr _retValue;

        public ReturnStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ReturnStmt);
        }

        public Expr RetValue => _retValue;

        protected override Expr GetOrAddExpr(CXCursor childHandle)
        {
            var expr = base.GetOrAddExpr(childHandle);

            Debug.Assert(_retValue is null);
            _retValue = expr;

            return expr;
        }
    }
}
