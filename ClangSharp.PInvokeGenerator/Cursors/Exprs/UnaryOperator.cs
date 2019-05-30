using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnaryOperator : Expr
    {
        private readonly Lazy<(string Opcode, bool IsPrefix)> _opcode;

        private Expr _subExpr;

        public UnaryOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnaryOperator);
            _opcode = new Lazy<(string Opcode, bool IsPrefix)>(GetOpcode);
        }

        public bool IsPrefix => _opcode.Value.IsPrefix;

        public string Opcode => _opcode.Value.Opcode;

        public Expr SubExpr => _subExpr;

        protected override Expr GetOrAddExpr(CXCursor childHandle)
        {
            var expr = base.GetOrAddExpr(childHandle);

            Debug.Assert(_subExpr is null);
            _subExpr = expr;

            return expr;
        }

        private (string Opcode, bool IsPrefix) GetOpcode()
        {
            var tokens = TranslationUnit.Tokenize(this);

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
