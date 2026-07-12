// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class LifetimeExtendedTemporaryDecl : Decl, IMergeable<LifetimeExtendedTemporaryDecl>
{
    private ValueLazy<LifetimeExtendedTemporaryDecl, ValueDecl> _extendingDecl;
    private ValueLazy<LifetimeExtendedTemporaryDecl, Expr> _temporaryExpr;

    internal unsafe LifetimeExtendedTemporaryDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_LifetimeExtendedTemporary)
    {
        _extendingDecl = new ValueLazy<LifetimeExtendedTemporaryDecl, ValueDecl>(&ExtendingDeclFactory);
        _temporaryExpr = new ValueLazy<LifetimeExtendedTemporaryDecl, Expr>(&TemporaryExprFactory);
    }

    public ValueDecl ExtendingDecl => _extendingDecl.GetValue(this);

    public Expr TemporaryExpr => _temporaryExpr.GetValue(this);

    private static unsafe Expr TemporaryExprFactory(LifetimeExtendedTemporaryDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.GetExpr(0));

    private static unsafe ValueDecl ExtendingDeclFactory(LifetimeExtendedTemporaryDecl self) => self.TranslationUnit.GetOrCreate<ValueDecl>(self.Handle.GetSubDecl(1));
}
