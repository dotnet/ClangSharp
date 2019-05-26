using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class PackExpansionExpr : Expr
    {
        public PackExpansionExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_PackExpansionExpr);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    return GetOrAddChild<DeclRefExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CallExpr:
                {
                    return GetOrAddChild<CallExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnaryExpr:
                {
                    return GetOrAddChild<UnaryExpr>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
