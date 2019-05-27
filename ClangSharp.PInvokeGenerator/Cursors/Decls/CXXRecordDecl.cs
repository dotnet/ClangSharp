namespace ClangSharp
{
    internal class CXXRecordDecl : RecordDecl
    {
        protected CXXRecordDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public bool IsAbstract => Handle.CXXRecord_IsAbstract;
    }
}
