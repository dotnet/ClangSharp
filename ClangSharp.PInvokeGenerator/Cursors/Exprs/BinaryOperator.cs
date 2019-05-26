using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class BinaryOperator : Expr
    {
        private readonly Lazy<string> _operator;

        private Expr _lhsExpr;
        private Expr _rhsExpr;

        public BinaryOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_BinaryOperator);

            _operator = new Lazy<string>(() => {
                var tokens = TranslationUnit.Tokenize(this);

                Debug.Assert(tokens.Length >= 3);

                var operatorIndex = GetOperatorIndex(tokens);
                Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);
                return tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public Expr LhsExpr
        {
            get
            {
                return _lhsExpr;
            }

            set
            {
                Debug.Assert((_lhsExpr is null) && (_rhsExpr is null));
                _lhsExpr = value;
            }
        }

        public string Operator => _operator.Value;

        public Expr RhsExpr
        {
            get
            {
                return _rhsExpr;
            }

            set
            {
                Debug.Assert((_lhsExpr != null) && (_rhsExpr is null));
                _rhsExpr = value;
            }
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsExpression)
            {
                var expr = GetOrAddChild<Expr>(childHandle);

                if (LhsExpr is null)
                {
                    LhsExpr = expr;
                }
                else
                {
                    RhsExpr = expr;
                }

                return expr.Visit(clientData);
            }

            return base.VisitChildren(childHandle, handle, clientData);
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
