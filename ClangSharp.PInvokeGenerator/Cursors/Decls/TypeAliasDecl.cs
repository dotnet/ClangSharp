using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TypeAliasDecl : Decl
    {
        public TypeAliasDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TypeAliasDecl);
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

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return GetOrAddChild<TypeRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TemplateRef:
                {
                    return GetOrAddChild<TemplateRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_NamespaceRef:
                {
                    return GetOrAddChild<NamespaceRef>(childHandle).Visit(clientData);
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

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    return GetOrAddChild<BinaryOperator>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ConditionalOperator:
                {
                    return GetOrAddChild<ConditionalOperator>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    return GetOrAddChild<CXXBoolLiteralExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnaryExpr:
                {
                    return GetOrAddChild<UnaryExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_PackExpansionExpr:
                {
                    return GetOrAddChild<PackExpansionExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_SizeOfPackExpr:
                {
                    return GetOrAddChild<SizeOfPackExpr>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
