// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class MaterializeTemporaryExpr : Expr
{
    private readonly Lazy<LifetimeExtendedTemporaryDecl> _lifetimeExtendedTemporaryDecl;

    internal MaterializeTemporaryExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_MaterializeTemporaryExpr)
    {
        _lifetimeExtendedTemporaryDecl = new Lazy<LifetimeExtendedTemporaryDecl>(() => TranslationUnit.GetOrCreate<LifetimeExtendedTemporaryDecl>(Handle.Referenced));
    }

    public ValueDecl ExtendingDecl => LifetimeExtendedTemporaryDecl.ExtendingDecl;

    public LifetimeExtendedTemporaryDecl LifetimeExtendedTemporaryDecl => _lifetimeExtendedTemporaryDecl.Value;

    public Expr SubExpr => (Expr)Children[0];
}
