// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class MaterializeTemporaryExpr : Expr
{
    private ValueLazy<MaterializeTemporaryExpr, LifetimeExtendedTemporaryDecl> _lifetimeExtendedTemporaryDecl;

    internal unsafe MaterializeTemporaryExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_MaterializeTemporaryExpr)
    {
        _lifetimeExtendedTemporaryDecl = new ValueLazy<MaterializeTemporaryExpr, LifetimeExtendedTemporaryDecl>(&LifetimeExtendedTemporaryDeclFactory);
    }

    public ValueDecl ExtendingDecl => LifetimeExtendedTemporaryDecl.ExtendingDecl;

    public LifetimeExtendedTemporaryDecl LifetimeExtendedTemporaryDecl => _lifetimeExtendedTemporaryDecl.GetValue(this);

    public Expr SubExpr => (Expr)Children[0];

    private static unsafe LifetimeExtendedTemporaryDecl LifetimeExtendedTemporaryDeclFactory(MaterializeTemporaryExpr self) => self.TranslationUnit.GetOrCreate<LifetimeExtendedTemporaryDecl>(self.Handle.Referenced);
}
