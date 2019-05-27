using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class DeclRefExpr : Expr
    {
        private readonly Lazy<string> _identifier;

        public DeclRefExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DeclRefExpr);

            _identifier = new Lazy<string>(() =>
            {
                var tokens = TranslationUnit.Tokenize(this);

                Debug.Assert(tokens.Length == 1);
                Debug.Assert(tokens[0].Kind == CXTokenKind.CXToken_Identifier);

                return tokens[0].GetSpelling(Handle.TranslationUnit).ToString();
            });
        }

        public string Identifier => _identifier.Value;

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

                case CXCursorKind.CXCursor_OverloadedDeclRef:
                {
                    return GetOrAddChild<OverloadedDeclRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    return GetOrAddChild<DeclRefExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return GetOrAddChild<IntegerLiteral>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    return GetOrAddChild<BinaryOperator>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_PackExpansionExpr:
                {
                    return GetOrAddChild<PackExpansionExpr>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
