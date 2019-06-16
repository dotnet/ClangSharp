using System;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class BinaryOperator : Expr
    {
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<string> _opcode;
        private readonly Lazy<Expr> _rhs;

        internal BinaryOperator(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            Debug.Assert(Children.Where((cursor) => cursor is Expr).Count() == 2);

            _lhs = new Lazy<Expr>(() => Children.Where((cursor) => cursor is Expr).Cast<Expr>().First());
            _opcode = new Lazy<string>(GetOpcode);
            _rhs = new Lazy<Expr>(() => Children.Where((cursor) => cursor is Expr).Cast<Expr>().Last());
        }

        public Expr LHS => _lhs.Value;

        public string Opcode => _opcode.Value;

        public Expr RHS => _rhs.Value;

        protected virtual string GetOpcode()
        {
            var tokens = Handle.TranslationUnit.Tokenize(Extent);

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
