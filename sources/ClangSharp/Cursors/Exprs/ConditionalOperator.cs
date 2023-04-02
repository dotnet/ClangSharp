// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ConditionalOperator : AbstractConditionalOperator
{
    internal ConditionalOperator(CXCursor handle) : base(handle, CXCursor_ConditionalOperator, CX_StmtClass_ConditionalOperator)
    {
        Debug.Assert(NumChildren is 3);
    }

    public new Expr Cond => (Expr)Children[0];

    public new Expr FalseExpr => RHS;

    public Expr LHS => (Expr)Children[1];

    public Expr RHS => (Expr)Children[2];

    public new Expr TrueExpr => LHS;
}
