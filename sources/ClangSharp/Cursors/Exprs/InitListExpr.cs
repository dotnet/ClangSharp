// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class InitListExpr : Expr
{
    private readonly Lazy<Expr> _arrayFiller;
    private readonly Lazy<FieldDecl> _initializedFieldInUnion;
    private readonly Lazy<IReadOnlyList<Expr>> _inits;

    internal InitListExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_InitListExpr, CX_StmtClass.CX_StmtClass_InitListExpr)
    {
        _arrayFiller = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
        _initializedFieldInUnion = new Lazy<FieldDecl>(() => TranslationUnit.GetOrCreate<FieldDecl>(Handle.Referenced));
        _inits = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
    }

    public Expr ArrayFiller => _arrayFiller.Value;

    public bool HasArrayFiller => ArrayFiller is not null;

    public FieldDecl InitializedFieldInUnion => _initializedFieldInUnion.Value;

    public IReadOnlyList<Stmt> Inits => _inits.Value;

    public bool IsExplicit => !Handle.IsImplicit;

    public bool IsTransparent => Handle.IsTransparent;

    public uint NumInits => NumChildren;
}
