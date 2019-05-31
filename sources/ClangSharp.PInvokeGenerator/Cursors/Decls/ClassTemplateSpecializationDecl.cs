namespace ClangSharp
{
    internal class ClassTemplateSpecializationDecl : CXXRecordDecl
    {
        protected ClassTemplateSpecializationDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public string DisplayName => Handle.DisplayName.ToString();

        public CXCursorKind TemplateKind => Handle.TemplateCursorKind;
    }
}
