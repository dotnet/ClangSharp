using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class BreakStmt : Stmt
    {
        public BreakStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_BreakStmt);
        }
    }
}
