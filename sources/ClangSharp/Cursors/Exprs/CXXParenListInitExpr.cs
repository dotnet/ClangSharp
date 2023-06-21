// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXParenListInitExpr : Expr
{
    private readonly Lazy<Cursor> _arrayFillerOrUnionFieldInit;
    private readonly Lazy<IReadOnlyList<Expr>> _initExprs;
    private readonly Lazy<IReadOnlyList<Expr>> _userSpecifiedInitExprs;

    internal CXXParenListInitExpr(CXCursor handle) : base(handle, CXCursor_CXXParenListInitExpr, CX_StmtClass_CXXParenListInitExpr)
    {
        _arrayFillerOrUnionFieldInit = new Lazy<Cursor>(() => TranslationUnit.GetOrCreate<Cursor>(Handle.SubExpr));
        _initExprs = new Lazy<IReadOnlyList<Expr>>(() => {
            var numInitExprs = Handle.NumExprs;
            var initExprs = new List<Expr>(numInitExprs);

            for (var i = 0; i < numInitExprs; i++)
            {
                var initExpr = TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i)));
                initExprs.Add(initExpr);
            }

            return initExprs;
        });
        _userSpecifiedInitExprs = new Lazy<IReadOnlyList<Expr>>(() => {
            var numUserSpecifiedInitExprs = Handle.NumExprsOther;
            var userSpecifiedInitExprs = new List<Expr>(numUserSpecifiedInitExprs);

            for (var i = 0; i < numUserSpecifiedInitExprs; i++)
            {
                var initExpr = TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i)));
                userSpecifiedInitExprs.Add(initExpr);
            }

            return userSpecifiedInitExprs;
        });
    }

    public Expr? ArrayFiller => _arrayFillerOrUnionFieldInit.Value as Expr;

    public IReadOnlyList<Expr> InitExprs => _initExprs.Value;

    public FieldDecl? InitializedFieldInUnion => _arrayFillerOrUnionFieldInit.Value as FieldDecl;

    public IReadOnlyList<Expr> UserSpecifiedInitExprs => _userSpecifiedInitExprs.Value;
}
