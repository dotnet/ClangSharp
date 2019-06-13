using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

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

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);
            _declarations.Add(decl);
            return decl;
        }
    }
}
