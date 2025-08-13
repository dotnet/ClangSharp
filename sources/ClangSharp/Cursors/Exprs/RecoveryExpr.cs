// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class RecoveryExpr : Expr
{
    private readonly LazyList<Expr, Stmt> _subExpressions;

    internal RecoveryExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_RecoveryExpr)
    {
        _subExpressions = LazyList.Create<Expr, Stmt>(_children);
    }

    public IReadOnlyList<Expr> SubExpressions => _subExpressions;
}
