using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class NamespaceDecl : NamedDecl
    {
        private readonly List<Decl> _declarations = new List<Decl>();

        public NamespaceDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_Namespace);
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
