namespace ClangSharp
{
    internal class TypedefNameDecl : TypeDecl
    {
        protected TypedefNameDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
