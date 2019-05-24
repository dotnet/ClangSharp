using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class ForStmt : Stmt
    {
        public ForStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ForStmt);
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

                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    return GetOrAddChild<CompoundStmt>(childHandle).Visit(clientData);
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
