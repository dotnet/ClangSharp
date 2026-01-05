// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class InitListExpr : Expr
{
    private readonly ValueLazy<Expr> _arrayFiller;
    private readonly ValueLazy<FieldDecl> _initializedFieldInUnion;
    private readonly LazyList<Expr, Stmt> _inits;

    internal InitListExpr(CXCursor handle) : base(handle, CXCursor_InitListExpr, CX_StmtClass_InitListExpr)
    {
        _arrayFiller = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
        _initializedFieldInUnion = new ValueLazy<FieldDecl>(() => TranslationUnit.GetOrCreate<FieldDecl>(Handle.Referenced));
        _inits = LazyList.Create<Expr, Stmt>(_children);
    }

    public Expr ArrayFiller => _arrayFiller.Value;

    public bool HasArrayFiller => ArrayFiller is not null;

    public FieldDecl InitializedFieldInUnion => _initializedFieldInUnion.Value;

    public IReadOnlyList<Stmt> Inits => _inits;

    public bool IsExplicit => !Handle.IsImplicit;

    public bool IsTransparent => Handle.IsTransparent;

    public uint NumInits => NumChildren;
}
