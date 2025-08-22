// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class PseudoObjectExpr : Expr
{
    private readonly LazyList<Expr, Stmt> _semantics;

    internal PseudoObjectExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_PseudoObjectExpr)
    {
        Debug.Assert(NumChildren >= 1);
        _semantics = LazyList.Create<Expr, Stmt>(_children, skip: 1);
    }

    public uint NumSemanticExprs => NumChildren - 1;

    public Expr? ResultExpr => (ResultExprIndex == 0) ? null : (Expr)Children[unchecked((int)ResultExprIndex)];

    public uint ResultExprIndex => unchecked((uint)Handle.ResultIndex);

    public IReadOnlyList<Expr> Semantics => _semantics;

    public Expr SyntacticForm => (Expr)Children[0];
}
