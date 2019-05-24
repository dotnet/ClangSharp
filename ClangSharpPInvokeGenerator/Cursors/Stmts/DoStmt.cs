using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class DoStmt : Stmt
    {
        public DoStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DoStmt);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    return GetOrAddChild<CXXBoolLiteralExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    return GetOrAddChild<CompoundStmt>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
