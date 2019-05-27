using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class StructDecl : Decl
    {
        private readonly List<FieldDecl> _fieldDecls;

        public StructDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_StructDecl);
            _fieldDecls = new List<FieldDecl>();
        }

        public IReadOnlyList<FieldDecl> FieldDecls => _fieldDecls;

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

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return GetOrAddChild<EnumDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    var fieldDecl = GetOrAddChild<FieldDecl>(childHandle);
                    _fieldDecls.Add(fieldDecl);
                    return fieldDecl.Visit(clientData);
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

                case CXCursorKind.CXCursor_FunctionTemplate:
                {
                    return GetOrAddChild<FunctionTemplate>(childHandle).Visit(clientData);
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

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return GetOrAddChild<TypeRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXBaseSpecifier:
                {
                    return GetOrAddChild<CXXBaseSpecifier>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_NamespaceRef:
                {
                    return GetOrAddChild<NamespaceRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnexposedExpr:
                {
                    return GetOrAddChild<UnexposedExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    return GetOrAddChild<CXXBoolLiteralExpr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return GetOrAddChild<UnexposedAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeAliasTemplateDecl:
                {
                    return GetOrAddChild<TypeAliasTemplateDecl>(childHandle).Visit(clientData);
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
