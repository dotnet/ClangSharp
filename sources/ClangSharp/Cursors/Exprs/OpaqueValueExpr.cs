// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class OpaqueValueExpr : Expr
{
    private ValueLazy<OpaqueValueExpr, Expr> _sourceExpr;

    internal unsafe OpaqueValueExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_OpaqueValueExpr)
    {
        _sourceExpr = new ValueLazy<OpaqueValueExpr, Expr>(&SourceExprFactory);
    }

    public Expr SourceExpr => _sourceExpr.GetValue(this);

    private static unsafe Expr SourceExprFactory(OpaqueValueExpr self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.SubExpr);
}
