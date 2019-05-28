using System;

namespace ClangSharp
{
    internal class CXXRecordDecl : RecordDecl
    {
        private readonly Lazy<Cursor> _specializedTemplate;

        public CXXRecordDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _specializedTemplate = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.SpecializedCursorTemplate, () => Create(handle.SpecializedCursorTemplate, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public bool IsAbstract => Handle.CXXRecord_IsAbstract;

        public Cursor SpecializedTemplate => _specializedTemplate.Value;
    }
}
