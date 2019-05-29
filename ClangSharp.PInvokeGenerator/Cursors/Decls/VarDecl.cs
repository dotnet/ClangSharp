using System;

namespace ClangSharp
{
    internal class VarDecl : DeclaratorDecl
    {
        private readonly Lazy<Cursor> _specializedTemplate;

        public VarDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _specializedTemplate = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.SpecializedCursorTemplate, () => Create(Handle.SpecializedCursorTemplate, this));
                cursor?.Visit(clientData: default);
                return cursor;
            });
        }

        public string Mangling => Handle.Mangling.ToString();

        public Cursor SpecializedTemplate => _specializedTemplate.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;

        public CXTLSKind TlsKind => Handle.TlsKind;
    }
}
