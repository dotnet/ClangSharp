using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ClassTemplatePartialSpecialization : Decl
    {
        public ClassTemplatePartialSpecialization(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ClassTemplatePartialSpecialization);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
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

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    return GetOrAddChild<ParmDecl>(childHandle).Visit(clientData);
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

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    return GetOrAddChild<TypeAliasDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXAccessSpecifier:
                {
                    return GetOrAddChild<CXXAccessSpecifier>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return GetOrAddChild<TypeRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TemplateRef:
                {
                    return GetOrAddChild<TemplateRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXBaseSpecifier:
                {
                    return GetOrAddChild<CXXBaseSpecifier>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnexposedExpr:
                {
                    return GetOrAddChild<UnexposedExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    return GetOrAddChild<DeclRefExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CallExpr:
                {
                    return GetOrAddChild<CallExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnaryOperator:
                {
                    return GetOrAddChild<UnaryOperator>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ConditionalOperator:
                {
                    return GetOrAddChild<ConditionalOperator>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXStaticCastExpr:
                {
                    return GetOrAddChild<CXXStaticCastExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    return GetOrAddChild<CXXBoolLiteralExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_PackExpansionExpr:
                {
                    return GetOrAddChild<PackExpansionExpr>(childHandle).Visit(clientData);
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
