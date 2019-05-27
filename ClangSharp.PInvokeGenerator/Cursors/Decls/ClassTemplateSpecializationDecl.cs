namespace ClangSharp
{
    internal class ClassTemplateSpecializationDecl : CXXRecordDecl
    {
        protected ClassTemplateSpecializationDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
