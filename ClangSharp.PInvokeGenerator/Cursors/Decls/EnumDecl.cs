using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class EnumDecl : Decl
    {
        private readonly List<EnumConstantDecl> _enumConstantDecls;
        private readonly Lazy<Type> _integerType;

        public EnumDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_EnumDecl);
            _enumConstantDecls = new List<EnumConstantDecl>();
            _integerType = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.EnumDecl_IntegerType, () => Type.Create(Handle.EnumDecl_IntegerType, TranslationUnit)));
        }

        public IReadOnlyList<EnumConstantDecl> EnumConstantDecls => _enumConstantDecls;

        public Type IntegerType => _integerType.Value;

        public bool IsScoped => Handle.EnumDecl_IsScoped;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.Kind == CXCursorKind.CXCursor_EnumConstantDecl)
            {
                var enumConstantDecl = GetOrAddChild<EnumConstantDecl>(childHandle);
                _enumConstantDecls.Add(enumConstantDecl);
                return enumConstantDecl.Visit(clientData);
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
