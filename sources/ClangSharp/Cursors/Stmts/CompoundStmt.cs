using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CompoundStmt : Stmt
    {
        private readonly List<Stmt> _body = new List<Stmt>();

        public CompoundStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CompoundStmt);
        }

        public IReadOnlyList<Stmt> Body => _body;

        protected override Expr GetOrAddExpr(CXCursor childHandle)
        {
            var expr = base.GetOrAddExpr(childHandle);
            _body.Add(expr);
            return expr;
        }

        protected override Stmt GetOrAddStmt(CXCursor childHandle)
        {
            var stmt = base.GetOrAddStmt(childHandle);
            _body.Add(stmt);
            return stmt;
        }
    }
}
