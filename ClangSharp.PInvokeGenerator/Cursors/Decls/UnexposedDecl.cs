using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnexposedDecl : Decl
    {
        private readonly List<Decl> _declarations = new List<Decl>();

        public UnexposedDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnexposedDecl);
        }

        public IReadOnlyList<Decl> Declarations => _declarations;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsDeclaration)
            {
                var decl = GetOrAddChild<Decl>(childHandle);
                _declarations.Add(decl);
                return decl.Visit(clientData);
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
