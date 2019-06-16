using ClangSharp.Interop;

namespace ClangSharp
{
    public class ValueStmt : Stmt
    {
        private protected ValueStmt(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
