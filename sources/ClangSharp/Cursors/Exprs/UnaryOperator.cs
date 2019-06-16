using System;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryOperator : Expr
    {
        private readonly Lazy<(string Opcode, bool IsPrefix)> _opcode;
        private readonly Lazy<Expr> _subExpr;

        internal UnaryOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryOperator)
        {
            _opcode = new Lazy<(string Opcode, bool IsPrefix)>(GetOpcode);
            _subExpr = new Lazy<Expr>(() => Children.Where((cursor) => cursor is Expr).Cast<Expr>().Single());
        }

        public bool IsPrefix => _opcode.Value.IsPrefix;

        public bool IsPostfix => !_opcode.Value.IsPrefix;

        public string Opcode => _opcode.Value.Opcode;

        public Expr SubExpr => _subExpr.Value;

        private (string Opcode, bool IsPrefix) GetOpcode()
        {
            var tokens = Handle.TranslationUnit.Tokenize(Extent);

            Debug.Assert(tokens.Length >= 2);

            int operatorIndex = -1;
            int parenDepth = 0;
            bool isPrefix = false;

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
                    case "!":
                    case "&":
                    case "*":
                    case "+":
                    case "-":
                    case "~":
                    {
                        if (parenDepth == 0)
                        {
                            operatorIndex = index;
                            isPrefix = true;
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

                    case "++":
                    case "--":
                    {
                        if (parenDepth == 0)
                        {
                            operatorIndex = index;
                            isPrefix = ((index + 1) != tokens.Length);
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
            Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);

            var opcode = tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
            return (opcode, isPrefix);
        }
    }
}
