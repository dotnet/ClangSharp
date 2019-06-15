using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class TagType : Type
    {
        private readonly Lazy<TagDecl> _decl;

        protected TagType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
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
