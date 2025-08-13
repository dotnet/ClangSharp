// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class OpaqueValueExpr : Expr
{
    private readonly ValueLazy<Expr> _sourceExpr;

    internal OpaqueValueExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_OpaqueValueExpr)
    {
        _sourceExpr = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.SubExpr));
    }

    public Expr SourceExpr => _sourceExpr.Value;
}
