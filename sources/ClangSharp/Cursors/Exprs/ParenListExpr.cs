// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ParenListExpr : Expr
{
    private readonly Lazy<IReadOnlyList<Expr>> _exprs;

    internal ParenListExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ParenListExpr)
    {
        _exprs = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
    }

    public IReadOnlyList<Expr> Exprs => _exprs.Value;

    public uint NumExprs => NumChildren;
}
