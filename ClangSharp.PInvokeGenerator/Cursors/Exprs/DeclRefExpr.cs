using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class DeclRefExpr : Expr
    {
        private readonly Lazy<string> _identifier;
        private readonly Lazy<Cursor> _referenced;

        public DeclRefExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DeclRefExpr);

            _identifier = new Lazy<string>(() =>
            {
                var tokens = TranslationUnit.Tokenize(this);

                Debug.Assert(tokens.Length == 1);
                Debug.Assert(tokens[0].Kind == CXTokenKind.CXToken_Identifier);

                return tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            });
            _referenced = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.Referenced, () => Create(handle.Referenced, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public string Identifier => _identifier.Value;

        public Cursor Referenced => _referenced.Value;

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => Handle.GetReferenceNameRange(nameFlags, pieceIndex);
    }
}
