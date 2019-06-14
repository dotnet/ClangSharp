using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class BinaryOperator : Expr
    {
        private readonly Lazy<string> _opcode;

        private Expr _lhs;
        private Expr _rhs;

        public BinaryOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _opcode = new Lazy<string>(GetOpcode);
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

        protected virtual string GetOpcode()
        {
            var tokens = TranslationUnit.Tokenize(this);

            Debug.Assert(tokens.Length >= 3);

            int operatorIndex = -1;
            int parenDepth = 0;

            for (int index = 0; (index < tokens.Length) && (operatorIndex == -1); index++)
            {
                var token = tokens[index];

                if (token.Kind != CXTokenKind.CXToken_Punctuation)
                {
                    continue;
                }

                var punctuation = tokens[index].GetSpelling(Handle.TranslationUnit).ToString();

                switch (punctuation)
                {
                    case "!=":
                    case "%":
                    case "&":
                    case "&&":
                    case "*":
                    case "+":
                    case "-":
                    case "/":
                    case "<":
                    case "<<":
                    case "<=":
                    case "=":
                    case "==":
                    case ">":
                    case ">>":
                    case ">=":
                    case "^":
                    case "|":
                    case "||":
                    {
                        if (parenDepth == 0)
                        {
                            operatorIndex = index;
                        }
                        break;
                    }

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

                    default:
                    {
                        Debug.WriteLine($"Unhandled punctuation kind: {punctuation}.");
                        Debugger.Break();
                        break;
                    }
                }
            }

            Debug.Assert(operatorIndex != -1);
            Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);
            return tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
        }
    }
}
