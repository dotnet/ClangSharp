namespace ClangSharp
{
    internal class RedeclarableTemplateDecl : TemplateDecl
    {
        protected RedeclarableTemplateDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
