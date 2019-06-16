using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DeclStmt : Stmt
    {
        internal DeclStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DeclStmt)
        {
        }
    }
}
