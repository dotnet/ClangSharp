using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IfStmt : Stmt
    {
        internal IfStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_IfStmt)
        {
        }
    }
}
