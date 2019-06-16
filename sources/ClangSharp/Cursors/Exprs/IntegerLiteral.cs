using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IntegerLiteral : Expr
    {
        private readonly Lazy<string> _value;

        internal IntegerLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_IntegerLiteral)
        {
            _value = new Lazy<string>(() => {
                var tokens = Handle.TranslationUnit.Tokenize(Extent);

                Debug.Assert(tokens.Length == 1);
                Debug.Assert(tokens[0].Kind == CXTokenKind.CXToken_Literal);

                return tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public string Value => _value.Value;
    }
}
