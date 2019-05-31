using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class DeclStmt : Stmt
    {
        public DeclStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DeclStmt);
        }
    }
}
