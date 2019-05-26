using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnaryOperator : Expr
    {
        private readonly Lazy<bool> _isPrefix;
        private readonly Lazy<string> _operator;

        private Expr _expr;

        public UnaryOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnaryOperator);

            _isPrefix = new Lazy<bool>(() => {
                switch (Operator)
                {
                    case "-":
                    {
                        return true;
                    }

                    default:
                    {
                        Debug.WriteLine($"Unhandled operator kind: {Operator}.");
                        Debugger.Break();
                        return false;
                    }
                }
            });

            _operator = new Lazy<string>(() => {
                var tokens = TranslationUnit.Tokenize(this);

                Debug.Assert(tokens.Length >= 2);

                var operatorIndex = GetOperatorIndex(tokens);
                Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);
                return tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public Expr Expr
        {
            get
            {
                return _expr;
            }

            set
            {
                Debug.Assert(_expr is null);
                _expr = value;
            }
        }

        public bool IsPrefix => _isPrefix.Value;

        public string Operator => _operator.Value;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsExpression)
            {
                var expr = GetOrAddChild<Expr>(childHandle);
                Expr = expr;
                return expr.Visit(clientData);
            }
            else
            {
                return base.VisitChildren(childHandle, handle, clientData);
            }
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
