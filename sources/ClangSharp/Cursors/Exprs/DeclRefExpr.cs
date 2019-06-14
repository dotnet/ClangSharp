using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DeclRefExpr : Expr
    {
        private readonly Lazy<ValueDecl> _decl;

        public DeclRefExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DeclRefExpr);

            _decl = new Lazy<ValueDecl>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.Referenced, () => ClangSharp.Decl.Create(Handle.Referenced, this));
                cursor?.Visit(clientData: default);
                return (ValueDecl)cursor;
            });
        }

        public ValueDecl Decl => _decl.Value;

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => Handle.GetReferenceNameRange(nameFlags, pieceIndex);
    }
}
