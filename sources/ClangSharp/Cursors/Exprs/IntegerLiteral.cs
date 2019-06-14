using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IntegerLiteral : Expr
    {
        private readonly Lazy<string> _value;

        public IntegerLiteral(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_IntegerLiteral);

            _value = new Lazy<string>(() => {
                var tokens = TranslationUnit.Tokenize(this);

                Debug.Assert(tokens.Length == 1);
                Debug.Assert(tokens[0].Kind == CXTokenKind.CXToken_Literal);

                return tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public string Value => _value.Value;
    }
}
