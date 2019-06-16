using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DeclRefExpr : Expr
    {
        private readonly Lazy<ValueDecl> _decl;

        internal DeclRefExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DeclRefExpr)
        {
            _decl = new Lazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.Referenced));
        }

        public ValueDecl Decl => _decl.Value;
    }
}
