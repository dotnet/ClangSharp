using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal class RedeclarableTemplateDecl : TemplateDecl
    {
        private readonly Lazy<Cursor> _specializedTemplate;

        protected RedeclarableTemplateDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _specializedTemplate = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.SpecializedCursorTemplate, () => Create(Handle.SpecializedCursorTemplate, this));
                cursor?.Visit(clientData: default);
                return cursor;
            });
        }

        public Cursor SpecializedTemplate => _specializedTemplate.Value;
    }
}
