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

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);
            _declarations.Add(decl);
            return decl;
        }
    }
}
