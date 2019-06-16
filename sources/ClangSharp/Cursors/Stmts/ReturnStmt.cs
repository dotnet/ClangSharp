using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ReturnStmt : Stmt
    {
        private readonly Lazy<Expr> _retValue;

        internal ReturnStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ReturnStmt)
        {
            _retValue = new Lazy<Expr>(() => Children.Where((stmt) => stmt is Expr).Cast<Expr>().Single());
        }

        public Expr RetValue => _retValue.Value;
    }
}
