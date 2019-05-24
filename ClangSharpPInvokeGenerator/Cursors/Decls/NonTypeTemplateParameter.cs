﻿using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class NonTypeTemplateParameter : Decl
    {
        public NonTypeTemplateParameter(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_NonTypeTemplateParameter);
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

                case CXCursorKind.CXCursor_UnexposedExpr:
                {
                    return GetOrAddChild<UnexposedExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    return GetOrAddChild<DeclRefExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return GetOrAddChild<IntegerLiteral>(childHandle).Visit(clientData);
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
