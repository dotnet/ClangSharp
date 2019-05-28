using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnaryOperator : Expr
    {
        private readonly Lazy<bool> _isPrefix;
        private readonly Lazy<string> _opcode;

        private Expr _subExpr;

        public UnaryOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnaryOperator);

            _isPrefix = new Lazy<bool>(() => {
                switch (Opcode)
                {
                    case "-":
                    {
                        return true;
                    }

                    default:
                    {
                        Debug.WriteLine($"Unhandled operator kind: {Opcode}.");
                        Debugger.Break();
                        return false;
                    }
                }
            });

            _opcode = new Lazy<string>(() => {
                var tokens = TranslationUnit.Tokenize(this);

                Debug.Assert(tokens.Length >= 2);

                var operatorIndex = GetOperatorIndex(tokens);
                Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);
                return tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public bool IsPrefix => _isPrefix.Value;

        public bool IsPostfix => !_isPrefix.Value;

        public string Opcode => _opcode.Value;

        public Expr SubExpr => _subExpr;

        protected override Expr GetOrAddExpr(CXCursor childHandle)
        {
            var expr = base.GetOrAddExpr(childHandle);

            Debug.Assert(_subExpr is null);
            _subExpr = expr;

            return expr;
        }

        private int GetOperatorIndex(CXToken[] tokens)
        {
            int operatorIndex = -1;
            int parenDepth = 0;

            for (int index = 0; index < tokens.Length; index++)
            {
                var token = tokens[index];

                if (token.Kind != CXTokenKind.CXToken_Punctuation)
                {
                    continue;
                }

                var punctuation = tokens[index].GetSpelling(Handle.TranslationUnit).ToString();

                switch (punctuation)
                {
                    case "(":
                    {
                        parenDepth++;
                        break;
                    }

                    case ")":
                    {
                        parenDepth--;
                        break;
                    }

                    case "-":
                    {
                        if (parenDepth == 0)
                        {
                            return index;
                        }

                        break;
                    }

                    default:
                    {
                        Debug.WriteLine($"Unhandled punctuation kind: {punctuation}.");
                        Debugger.Break();
                        break;
                    }
                }
            }

            Debug.Assert(operatorIndex != -1);
            return operatorIndex;
        }
    }
}
