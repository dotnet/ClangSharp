using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class ArraySubscriptExpr : Expr
    {
        public ArraySubscriptExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ArraySubscriptExpr);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedExpr:
                {
                    return GetOrAddChild<UnexposedExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    return GetOrAddChild<DeclRefExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_MemberRefExpr:
                {
                    return GetOrAddChild<MemberRefExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return GetOrAddChild<IntegerLiteral>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXStaticCastExpr:
                {
                    return GetOrAddChild<CXXStaticCastExpr>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
