﻿using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class MemberExpr : Expr
    {
        public MemberExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_MemberRefExpr);
        }

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => Handle.GetReferenceNameRange(nameFlags, pieceIndex);
    }
}
