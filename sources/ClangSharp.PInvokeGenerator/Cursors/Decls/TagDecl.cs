using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal class TagDecl : TypeDecl
    {
        private readonly List<Decl> _declarations = new List<Decl>();
        private readonly Lazy<TagDecl> _definition;

        protected TagDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _definition = new Lazy<TagDecl>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.Definition, () => Create(Handle.Definition, this));
                cursor?.Visit(clientData: default);
                return (TagDecl)cursor;
            });
        }

        public IReadOnlyList<Decl> Declarations => _declarations;

        public TagDecl Definition => _definition.Value;

        public bool IsAnonymous => Handle.IsAnonymous;

        public bool IsDefinition => Handle.IsDefinition;

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);
            _declarations.Add(decl);
            return decl;
        }
    }
}
