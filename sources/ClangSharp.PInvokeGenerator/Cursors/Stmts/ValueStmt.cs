namespace ClangSharp
{
    internal class ValueStmt : Stmt
    {
        protected ValueStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
