namespace ClangSharp
{
    internal class VarDecl : DeclaratorDecl
    {
        public VarDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
