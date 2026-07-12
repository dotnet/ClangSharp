// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class CXXParenListInitExpr : Expr
{
    private ValueLazy<CXXParenListInitExpr, Cursor> _arrayFillerOrUnionFieldInit;
    private readonly LazyList<Expr> _initExprs;
    private readonly LazyList<Expr> _userSpecifiedInitExprs;

    internal unsafe CXXParenListInitExpr(CXCursor handle) : base(handle, CXCursor_CXXParenListInitExpr, CX_StmtClass_CXXParenListInitExpr)
    {
        _arrayFillerOrUnionFieldInit = new ValueLazy<CXXParenListInitExpr, Cursor>(&ArrayFillerOrUnionFieldInitFactory);
        _initExprs = LazyList.Create<Expr>(Handle.NumExprs, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i))));
        _userSpecifiedInitExprs = LazyList.Create<Expr>(Handle.NumExprsOther, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i))));
    }

    public Expr? ArrayFiller => _arrayFillerOrUnionFieldInit.GetValue(this) as Expr;

    public IReadOnlyList<Expr> InitExprs => _initExprs;

    public FieldDecl? InitializedFieldInUnion => _arrayFillerOrUnionFieldInit.GetValue(this) as FieldDecl;

    public IReadOnlyList<Expr> UserSpecifiedInitExprs => _userSpecifiedInitExprs;

    private static unsafe Cursor ArrayFillerOrUnionFieldInitFactory(CXXParenListInitExpr self) => self.TranslationUnit.GetOrCreate<Cursor>(self.Handle.SubExpr);
}
