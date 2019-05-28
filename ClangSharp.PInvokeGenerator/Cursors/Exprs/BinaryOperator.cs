using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class BinaryOperator : Expr
    {
        private readonly Lazy<string> _opcode;

        private Expr _lhs;
        private Expr _rhs;

        public BinaryOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_BinaryOperator);

            _opcode = new Lazy<string>(() => {
                var tokens = TranslationUnit.Tokenize(this);

                Debug.Assert(tokens.Length >= 3);

                var operatorIndex = GetOperatorIndex(tokens);
                Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);
                return tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public Expr LHS => _lhs;

        public string Opcode => _opcode.Value;

        public Expr RHS => _rhs;

        protected override Expr GetOrAddExpr(CXCursor childHandle)
        {
            var expr = base.GetOrAddExpr(childHandle);

            if (_lhs is null)
            {
                _lhs = expr;
            }
            else
            {
                Debug.Assert(_rhs is null);
                _rhs = expr;
            }

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

                    case "|":
                    case "<<":
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
