using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class TagType : Type
    {
        private readonly Lazy<TagDecl> _decl;

        private protected TagType(CXType handle, CXTypeKind expectedKind) : base(handle, expectedKind)
        {
            _decl = new Lazy<TagDecl>(() => TranslationUnit.GetOrCreate<TagDecl>(Handle.Declaration));
        }

        public TagDecl Decl => _decl.Value;
    }
}
