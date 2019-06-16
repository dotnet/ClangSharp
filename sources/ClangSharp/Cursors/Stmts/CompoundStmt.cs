using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CompoundStmt : Stmt
    {
        public CompoundStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CompoundStmt)
        {
        }

        public IReadOnlyList<Stmt> Body => Children;
    }
}
