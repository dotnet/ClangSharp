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

            if (childHandle.IsDeclaration)
            {
                switch (childHandle.Kind)
                {
                    case CXCursorKind.CXCursor_ParmDecl:
                    {
                        var parmDecl = GetOrAddChild<ParmDecl>(childHandle);
                        parmDecl.Index = _parmDecls.Count;
                        _parmDecls.Add(parmDecl);
                        return parmDecl.Visit(clientData);
                    }
                }
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
