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

        public bool IsAbstract => Handle.CXXRecord_IsAbstract;

        public IReadOnlyList<FieldDecl> FieldDecls => _fieldDecls;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsDeclaration)
            {
                switch (childHandle.Kind)
                {
                    case CXCursorKind.CXCursor_FieldDecl:
                    {
                        var fieldDecl = GetOrAddChild<FieldDecl>(childHandle);
                        _fieldDecls.Add(fieldDecl);
                        return fieldDecl.Visit(clientData);
                    }
                }
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
