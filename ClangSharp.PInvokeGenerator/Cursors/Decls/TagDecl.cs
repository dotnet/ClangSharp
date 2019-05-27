namespace ClangSharp
{
    internal class TagDecl : TypeDecl
    {
        protected TagDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public bool IsAnonymous => Handle.IsAnonymous;
    }
}
