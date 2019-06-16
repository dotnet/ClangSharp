using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class RedeclarableTemplateDecl : TemplateDecl, IRedeclarable<RedeclarableTemplateDecl>
    {
        private readonly Lazy<RedeclarableTemplateDecl> _instantiatedFromMemberTemplate;

        private protected RedeclarableTemplateDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _instantiatedFromMemberTemplate = new Lazy<RedeclarableTemplateDecl>(() => TranslationUnit.GetOrCreate<RedeclarableTemplateDecl>(Handle.SpecializedCursorTemplate));
        }

        public RedeclarableTemplateDecl InstantiatedFromMemberTemplate => _instantiatedFromMemberTemplate.Value;
    }
}
