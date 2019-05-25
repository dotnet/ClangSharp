using System;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
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

                case CXCursorKind.CXCursor_MemberRefExpr:
                {
                    expr = GetOrAddChild<MemberRefExpr>(childHandle);
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

                case CXCursorKind.CXCursor_UnaryOperator:
                {
                    expr = GetOrAddChild<UnaryOperator>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_ArraySubscriptExpr:
                {
                    expr = GetOrAddChild<ArraySubscriptExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    expr = GetOrAddChild<BinaryOperator>(childHandle);
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

                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    expr = GetOrAddChild<CXXBoolLiteralExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_UnaryExpr:
                {
                    expr = GetOrAddChild<UnaryExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_SizeOfPackExpr:
                {
                    expr = GetOrAddChild<SizeOfPackExpr>(childHandle);
                    break;
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }

            Debug.Assert(expr != null);

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
