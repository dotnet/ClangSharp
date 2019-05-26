using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class IntegerLiteral : Expr
    {
        private readonly Lazy<string> _rawValue;

        public IntegerLiteral(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_IntegerLiteral);

            _rawValue = new Lazy<string>(() => {
                var tokens = TranslationUnit.Tokenize(this);

                Debug.Assert(tokens.Length == 1);
                Debug.Assert(tokens[0].Kind == CXTokenKind.CXToken_Literal);

                return tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public string RawValue => _rawValue.Value;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
