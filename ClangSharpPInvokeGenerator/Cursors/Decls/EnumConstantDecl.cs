using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class EnumConstantDecl : Decl
    {
        private Expr _expr;

        public EnumConstantDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_EnumConstantDecl);
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

        public ulong UnsignedValue => Handle.EnumConstantDeclUnsignedValue;

        public long Value => Handle.EnumConstantDeclValue;

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

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    expr = GetOrAddChild<IntegerLiteral>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_CharacterLiteral:
                {
                    expr = GetOrAddChild<CharacterLiteral>(childHandle);
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

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    expr = GetOrAddChild<BinaryOperator>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_ConditionalOperator:
                {
                    expr = GetOrAddChild<ConditionalOperator>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_CStyleCastExpr:
                {
                    expr = GetOrAddChild<CStyleCastExpr>(childHandle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    expr = GetOrAddChild<CXXBoolLiteralExpr>(childHandle);
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
    }
}
