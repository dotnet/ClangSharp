// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class LifetimeExtendedTemporaryDecl : Decl, IMergeable<LifetimeExtendedTemporaryDecl>
{
    private readonly ValueLazy<ValueDecl> _extendingDecl;
    private readonly ValueLazy<Expr> _temporaryExpr;

    internal LifetimeExtendedTemporaryDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_LifetimeExtendedTemporary)
    {
        _extendingDecl = new ValueLazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.GetSubDecl(1)));
        _temporaryExpr = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
    }

    public ValueDecl ExtendingDecl => _extendingDecl.Value;

    public Expr TemporaryExpr => _temporaryExpr.Value;
}
