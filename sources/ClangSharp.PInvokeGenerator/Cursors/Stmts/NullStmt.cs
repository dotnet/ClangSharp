﻿using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class NullStmt : Stmt
    {
        public NullStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_NullStmt);
        }
    }
}
