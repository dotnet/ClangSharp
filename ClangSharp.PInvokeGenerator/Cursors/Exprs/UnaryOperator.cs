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

            Expr expr;

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedExpr:
                {
                    expr = GetOrAddChild<UnexposedExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    expr = GetOrAddChild<DeclRefExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_CallExpr:
                {
                    expr = GetOrAddChild<CallExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    expr = GetOrAddChild<IntegerLiteral>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_FloatingLiteral:
                {
                    expr = GetOrAddChild<FloatingLiteral>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_ParenExpr:
                {
                    expr = GetOrAddChild<ParenExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_CStyleCastExpr:
                {
                    expr = GetOrAddChild<CStyleCastExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXStaticCastExpr:
                {
                    expr = GetOrAddChild<CXXStaticCastExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXThisExpr:
                {
                    expr = GetOrAddChild<CXXThisExpr>(childHandle);
                    break;
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }

            Debug.Assert(expr != null);
            Expr = expr;
            return expr.Visit(clientData);
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
