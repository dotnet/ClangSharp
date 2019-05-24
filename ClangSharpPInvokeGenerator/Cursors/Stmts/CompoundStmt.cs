using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class CompoundStmt : Stmt
    {
        public CompoundStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CompoundStmt);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_CallExpr:
                {
                    return GetOrAddChild<CallExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnaryOperator:
                {
                    return GetOrAddChild<UnaryOperator>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    return GetOrAddChild<BinaryOperator>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CStyleCastExpr:
                {
                    return GetOrAddChild<CStyleCastExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_IfStmt:
                {
                    return GetOrAddChild<IfStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DoStmt:
                {
                    return GetOrAddChild<DoStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ForStmt:
                {
                    return GetOrAddChild<ForStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_BreakStmt:
                {
                    return GetOrAddChild<BreakStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ReturnStmt:
                {
                    return GetOrAddChild<ReturnStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_NullStmt:
                {
                    return GetOrAddChild<NullStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DeclStmt:
                {
                    return GetOrAddChild<DeclStmt>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
