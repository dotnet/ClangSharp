using System;

namespace ClangSharp
{
    internal class RedeclarableTemplateDecl : TemplateDecl
    {
        private readonly Lazy<Cursor> _specializedTemplate;

        protected RedeclarableTemplateDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _specializedTemplate = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.SpecializedCursorTemplate, () => Create(handle.SpecializedCursorTemplate, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public Cursor SpecializedTemplate => _specializedTemplate.Value;
    }
}
