using ClangSharp.Interop;

namespace ClangSharp
{
    public class ValueStmt : Stmt
    {
        protected ValueStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
