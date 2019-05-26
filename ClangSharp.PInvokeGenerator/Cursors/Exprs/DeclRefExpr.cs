using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class DeclRefExpr : Expr
    {
        private readonly Lazy<string> _identifier;

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
        }

        public string Identifier => _identifier.Value;
    }
}
