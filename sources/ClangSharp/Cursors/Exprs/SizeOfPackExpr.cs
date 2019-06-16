using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SizeOfPackExpr : Expr
    {
        private readonly Lazy<NamedDecl> _pack;

        internal SizeOfPackExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SizeOfPackExpr)
        {
            _pack = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.Referenced));
        }

        public NamedDecl Pack => _pack.Value;
    }
}
