using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class IfStmt : Stmt
    {
        public IfStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_IfStmt);
        }
    }
}
