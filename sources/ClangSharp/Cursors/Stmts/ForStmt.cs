using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ForStmt : Stmt
    {
        internal ForStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ForStmt)
        {
        }
    }
}
