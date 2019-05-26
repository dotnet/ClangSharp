using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXMethod : Decl
    {
        public CXXMethod(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXMethod);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_ParmDecl:
                {
                    return GetOrAddChild<ParmDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TemplateTypeParameter:
                {
                    return GetOrAddChild<TemplateTypeParameter>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return GetOrAddChild<TypeRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TemplateRef:
                {
                    return GetOrAddChild<TemplateRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    return GetOrAddChild<CompoundStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return GetOrAddChild<UnexposedAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DLLExport:
                {
                    return GetOrAddChild<DLLExport>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DLLImport:
                {
                    return GetOrAddChild<DLLImport>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
