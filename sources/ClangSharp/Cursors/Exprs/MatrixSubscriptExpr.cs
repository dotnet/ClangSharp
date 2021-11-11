// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MatrixSubscriptExpr : Expr
    {
        internal MatrixSubscriptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_MatrixSubscriptExpr)
        {
            Debug.Assert(NumChildren is 3);
        }

        public Expr Base => (Expr)Children[0];

        public Expr ColumnIdx => IsIncomplete ? null : (Expr)Children[2];

        public bool IsIncomplete => Handle.IsIncomplete;

        public Expr RowIdx => (Expr)Children[1];
    }
}
