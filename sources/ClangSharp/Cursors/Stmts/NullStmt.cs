using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NullStmt : Stmt
    {
        internal NullStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_NullStmt)
        {
        }
    }
}
