using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DeclStmt : Stmt
    {
        public DeclStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DeclStmt);
        }
    }
}
