using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IfStmt : Stmt
    {
        public IfStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_IfStmt);
        }
    }
}
