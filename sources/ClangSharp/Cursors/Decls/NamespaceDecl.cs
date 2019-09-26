using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NamespaceDecl : NamedDecl, IDeclContext, IRedeclarable<NamespaceDecl>
    {
        private readonly Lazy<IReadOnlyList<Decl>> _decls;

        internal NamespaceDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Namespace)
        {
            _decls = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.Where((cursor) => cursor is Decl).Cast<Decl>().ToList());
        }

        public bool IsAnonymousNamespace => Handle.IsAnonymous;

        public bool IsInlineNamespace => Handle.IsInlineNamespace;

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public IDeclContext LexicalParent => LexicalDeclContext;

        public IDeclContext Parent => DeclContext;
    }
}
