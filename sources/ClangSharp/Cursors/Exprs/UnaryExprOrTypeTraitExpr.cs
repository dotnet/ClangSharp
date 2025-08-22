// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class UnaryExprOrTypeTraitExpr : Expr
{
    private readonly ValueLazy<Expr> _argumentExpr;
    private readonly ValueLazy<Type> _argumentType;

    internal UnaryExprOrTypeTraitExpr(CXCursor handle) : base(handle, CXCursor_UnaryExpr, CX_StmtClass_UnaryExprOrTypeTraitExpr)
    {
        _argumentExpr = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
        _argumentType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ArgumentType));
    }

    public Expr ArgumentExpr => _argumentExpr.Value;

    public Type ArgumentType => _argumentType.Value;

    public bool IsArgumentType => Handle.IsArgumentType;

    public CX_UnaryExprOrTypeTrait Kind => Handle.UnaryExprOrTypeTraitKind;

    public Type TypeOfArgument => IsArgumentType ? ArgumentType : ArgumentExpr.Type;
}
