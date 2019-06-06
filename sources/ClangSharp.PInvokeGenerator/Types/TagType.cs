using System;

namespace ClangSharp
{
    internal class TagType : Type
    {
        private readonly Lazy<TagDecl> _decl;

        protected TagType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            _decl = new Lazy<TagDecl>(() => {
                var cursor = translationUnit.GetOrCreateCursor(Handle.Declaration, () => Cursor.Create(Handle.Declaration, translationUnit));
                cursor?.Visit(clientData: default);
                return (TagDecl)cursor;
            });
        }

        public TagDecl Decl => _decl.Value;
    }
}
