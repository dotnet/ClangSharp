namespace ClangSharp
{
    internal class NamedDecl : Decl
    {
        protected NamedDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
