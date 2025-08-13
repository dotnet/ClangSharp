// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class GenericSelectionExpr : Expr
{
    private readonly LazyList<Expr, Stmt> _assocExprs;

    internal GenericSelectionExpr(CXCursor handle) : base(handle, CXCursor_GenericSelectionExpr, CX_StmtClass_GenericSelectionExpr)
    {
        _assocExprs = LazyList.Create<Expr, Stmt>(_children, skip: 1, take: (int)NumAssocs);
    }

    public IReadOnlyList<Expr> AssocExprs => _assocExprs;

    public Expr ControllingExpr => (Expr)Children[0];

    public bool IsResultDependent => Handle.IsResultDependent;

    public uint NumAssocs => unchecked((uint)Handle.NumAssocs);

    public Expr? ResultExpr => IsResultDependent ? null : (Expr)Children[(int)ResultIndex];

    public uint ResultIndex => unchecked((uint)Handle.ResultIndex);
}
