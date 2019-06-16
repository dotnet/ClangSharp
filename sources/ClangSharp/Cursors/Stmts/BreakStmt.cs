using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BreakStmt : Stmt
    {
        internal BreakStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_BreakStmt)
        {
        }
    }
}
