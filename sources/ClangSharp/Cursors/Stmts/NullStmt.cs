using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NullStmt : Stmt
    {
        public NullStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_NullStmt);
        }
    }
}
