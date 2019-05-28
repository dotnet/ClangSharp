using System.Collections.Generic;

namespace ClangSharp
{
    internal class TagDecl : TypeDecl
    {
        private readonly List<Decl> _declarations = new List<Decl>();

        protected TagDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public bool IsAnonymous => Handle.IsAnonymous;

        public IReadOnlyList<Decl> Declarations => _declarations;

        protected TDecl GetOrAddDecl<TDecl>(CXCursor childHandle)
            where TDecl : Decl
        {
            var decl = GetOrAddChild<Decl>(childHandle);
            _declarations.Add(decl);
            return (TDecl)decl;
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsDeclaration)
            {
                return GetOrAddDecl<Decl>(childHandle).Visit(clientData);
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
