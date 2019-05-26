using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TypedefDecl : Decl
    {
        private readonly List<ParmDecl> _parmDecls;
        private readonly Lazy<Type> _underlyingType;

        public TypedefDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TypedefDecl);
            _parmDecls = new List<ParmDecl>();
            _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.TypedefDeclUnderlyingType, () => Type.Create(Handle.TypedefDeclUnderlyingType, TranslationUnit)));
        }

        public IReadOnlyList<ParmDecl> ParmDecls => _parmDecls;

        public Type UnderlyingType => _underlyingType.Value;

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

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    var parmDecl = GetOrAddChild<ParmDecl>(childHandle);
                    parmDecl.Index = _parmDecls.Count;
                    _parmDecls.Add(parmDecl);
                    return parmDecl.Visit(clientData);
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

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return GetOrAddChild<IntegerLiteral>(childHandle).Visit(clientData);
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

                case CXCursorKind.CXCursor_CXXNullPtrLiteralExpr:
                {
                    return GetOrAddChild<CXXNullPtrLiteralExpr>(childHandle).Visit(clientData);
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
