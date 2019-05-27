﻿using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ReturnStmt : Stmt
    {
        public ReturnStmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ReturnStmt);
        }
    }
}
