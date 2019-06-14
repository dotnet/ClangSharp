using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ForStmt : Stmt
    {
        public ForStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ForStmt);
        }
    }
}
