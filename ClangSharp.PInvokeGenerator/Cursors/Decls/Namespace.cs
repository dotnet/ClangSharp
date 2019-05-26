using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class Namespace : Decl
    {
        public Namespace(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_Namespace);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                {
                    return GetOrAddChild<UnexposedDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_StructDecl:
                {
                    return GetOrAddChild<StructDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ClassDecl:
                {
                    return GetOrAddChild<ClassDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return GetOrAddChild<EnumDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    return GetOrAddChild<FunctionDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_VarDecl:
                {
                    return GetOrAddChild<VarDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    return GetOrAddChild<TypedefDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXMethod:
                {
                    return GetOrAddChild<CXXMethod>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_Namespace:
                {
                    return GetOrAddChild<Namespace>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_Constructor:
                {
                    return GetOrAddChild<Constructor>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FunctionTemplate:
                {
                    return GetOrAddChild<FunctionTemplate>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ClassTemplate:
                {
                    return GetOrAddChild<ClassTemplate>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ClassTemplatePartialSpecialization:
                {
                    return GetOrAddChild<ClassTemplatePartialSpecialization>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UsingDeclaration:
                {
                    return GetOrAddChild<UsingDeclaration>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    return GetOrAddChild<TypeAliasDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return GetOrAddChild<UnexposedAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeAliasTemplateDecl:
                {
                    return GetOrAddChild<TypeAliasTemplateDecl>(childHandle).Visit(clientData);
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
