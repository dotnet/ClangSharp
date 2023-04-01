// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ChooseExpr : Expr
{
    internal ChooseExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ChooseExpr)
    {
        Debug.Assert(NumChildren is 3);
    }

    public Expr? ChosenSubExpr => IsConditionDependent ? (IsConditionTrue ? LHS : RHS) : null;

    public Expr Cond => (Expr)Children[0];

    public bool IsConditionTrue => Handle.IsConditionTrue;

    public bool IsConditionDependent => Cond.IsTypeDependent || Cond.IsValueDependent;

    public Expr LHS => (Expr)Children[1];

    public Expr RHS => (Expr)Children[2];
}
