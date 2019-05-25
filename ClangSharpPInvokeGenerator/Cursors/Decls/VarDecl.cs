using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class VarDecl : Decl
    {
        public VarDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_VarDecl);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
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

                case CXCursorKind.CXCursor_MemberRefExpr:
                {
                    return GetOrAddChild<MemberRefExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CallExpr:
                {
                    return GetOrAddChild<CallExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return GetOrAddChild<IntegerLiteral>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FloatingLiteral:
                {
                    return GetOrAddChild<FloatingLiteral>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_StringLiteral:
                {
                    return GetOrAddChild<StringLiteral>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CharacterLiteral:
                {
                    return GetOrAddChild<CharacterLiteral>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ParenExpr:
                {
                    return GetOrAddChild<ParenExpr>(childHandle).Visit(clientData);
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

                case CXCursorKind.CXCursor_CStyleCastExpr:
                {
                    return GetOrAddChild<CStyleCastExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_InitListExpr:
                {
                    return GetOrAddChild<InitListExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXStaticCastExpr:
                {
                    return GetOrAddChild<CXXStaticCastExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    return GetOrAddChild<CXXBoolLiteralExpr>(childHandle).Visit(clientData);
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
