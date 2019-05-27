﻿using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXBoolLiteralExpr : Expr
    {
        public CXXBoolLiteralExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXBoolLiteralExpr);
        }
    }
}
