// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class RecoveryExpr : Expr
{
    private readonly Lazy<IReadOnlyList<Expr>> _subExpressions;

    internal RecoveryExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_RecoveryExpr)
    {
        _subExpressions = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
    }

    public IReadOnlyList<Expr> SubExpressions => _subExpressions.Value;
}
