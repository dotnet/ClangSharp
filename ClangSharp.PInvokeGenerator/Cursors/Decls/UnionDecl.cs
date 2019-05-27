using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnionDecl : Decl
    {
        public UnionDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnionDecl);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_StructDecl:
                {
                    return GetOrAddChild<StructDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnionDecl:
                {
                    return GetOrAddChild<UnionDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    return GetOrAddChild<FieldDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_Constructor:
                {
                    return GetOrAddChild<Constructor>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_Destructor:
                {
                    return GetOrAddChild<Destructor>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return GetOrAddChild<UnexposedAttr>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
