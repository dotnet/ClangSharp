using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ClassTemplate : Decl
    {
        public ClassTemplate(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ClassTemplate);
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

                case CXCursorKind.CXCursor_UnionDecl:
                {
                    return GetOrAddChild<UnionDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ClassDecl:
                {
                    return GetOrAddChild<ClassDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return GetOrAddChild<EnumDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    return GetOrAddChild<FieldDecl>(childHandle).Visit(clientData);
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

                case CXCursorKind.CXCursor_Constructor:
                {
                    return GetOrAddChild<Constructor>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_Destructor:
                {
                    return GetOrAddChild<Destructor>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ConversionFunction:
                {
                    return GetOrAddChild<ConversionFunction>(childHandle).Visit(clientData);
                }

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

                case CXCursorKind.CXCursor_FunctionTemplate:
                {
                    return GetOrAddChild<FunctionTemplate>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ClassTemplate:
                {
                    return GetOrAddChild<ClassTemplate>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UsingDeclaration:
                {
                    return GetOrAddChild<UsingDeclaration>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    return GetOrAddChild<TypeAliasDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXAccessSpecifier:
                {
                    return GetOrAddChild<CXXAccessSpecifier>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXBaseSpecifier:
                {
                    return GetOrAddChild<CXXBaseSpecifier>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return GetOrAddChild<UnexposedAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXFinalAttr:
                {
                    return GetOrAddChild<CXXFinalAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeAliasTemplateDecl:
                {
                    return GetOrAddChild<TypeAliasTemplateDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_StaticAssert:
                {
                    return GetOrAddChild<StaticAssert>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FriendDecl:
                {
                    return GetOrAddChild<FriendDecl>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
