using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DoStmt : Stmt
    {
        internal DoStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DoStmt)
        {
        }
    }
}
