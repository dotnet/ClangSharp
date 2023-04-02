// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class MatrixSubscriptExpr : Expr
{
    internal MatrixSubscriptExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_MatrixSubscriptExpr)
    {
        Debug.Assert(NumChildren is 3);
    }

    public Expr Base => (Expr)Children[0];

    public Expr? ColumnIdx => IsIncomplete ? null : (Expr)Children[2];

    public bool IsIncomplete => Handle.IsIncomplete;

    public Expr RowIdx => (Expr)Children[1];
}
