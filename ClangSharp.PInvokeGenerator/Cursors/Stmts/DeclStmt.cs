using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class DeclStmt : Stmt
    {
        public DeclStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DeclStmt);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_VarDecl:
                {
                    return GetOrAddChild<VarDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    return GetOrAddChild<TypeAliasDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_StaticAssert:
                {
                    return GetOrAddChild<StaticAssert>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
