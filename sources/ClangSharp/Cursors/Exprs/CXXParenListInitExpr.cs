// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class CXXParenListInitExpr : Expr
{
    private readonly ValueLazy<Cursor> _arrayFillerOrUnionFieldInit;
    private readonly LazyList<Expr> _initExprs;
    private readonly LazyList<Expr> _userSpecifiedInitExprs;

    internal CXXParenListInitExpr(CXCursor handle) : base(handle, CXCursor_CXXParenListInitExpr, CX_StmtClass_CXXParenListInitExpr)
    {
        _arrayFillerOrUnionFieldInit = new ValueLazy<Cursor>(() => TranslationUnit.GetOrCreate<Cursor>(Handle.SubExpr));
        _initExprs = LazyList.Create<Expr>(Handle.NumExprs, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i))));
        _userSpecifiedInitExprs = LazyList.Create<Expr>(Handle.NumExprsOther, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i))));
    }

    public Expr? ArrayFiller => _arrayFillerOrUnionFieldInit.Value as Expr;

    public IReadOnlyList<Expr> InitExprs => _initExprs;

    public FieldDecl? InitializedFieldInUnion => _arrayFillerOrUnionFieldInit.Value as FieldDecl;

    public IReadOnlyList<Expr> UserSpecifiedInitExprs => _userSpecifiedInitExprs;
}
