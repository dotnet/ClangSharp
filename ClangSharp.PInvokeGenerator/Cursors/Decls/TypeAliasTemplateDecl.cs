using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TypeAliasTemplateDecl : Decl
    {
        public TypeAliasTemplateDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TypeAliasTemplateDecl);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_TemplateTypeParameter:
                {
                    return GetOrAddChild<TemplateTypeParameter>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_NonTypeTemplateParameter:
                {
                    return GetOrAddChild<NonTypeTemplateParameter>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TemplateTemplateParameter:
                {
                    return GetOrAddChild<TemplateTemplateParameter>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    return GetOrAddChild<TypeAliasDecl>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
