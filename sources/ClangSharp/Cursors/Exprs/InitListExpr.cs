// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class InitListExpr : Expr
{
    private ValueLazy<InitListExpr, Expr> _arrayFiller;
    private ValueLazy<InitListExpr, FieldDecl> _initializedFieldInUnion;
    private readonly LazyList<Expr, Stmt> _inits;

    internal unsafe InitListExpr(CXCursor handle) : base(handle, CXCursor_InitListExpr, CX_StmtClass_InitListExpr)
    {
        _arrayFiller = new ValueLazy<InitListExpr, Expr>(&ArrayFillerFactory);
        _initializedFieldInUnion = new ValueLazy<InitListExpr, FieldDecl>(&InitializedFieldInUnionFactory);
        _inits = LazyList.Create<Expr, Stmt>(_children);
    }

    public Expr ArrayFiller => _arrayFiller.GetValue(this);

    public bool HasArrayFiller => ArrayFiller is not null;

    public FieldDecl InitializedFieldInUnion => _initializedFieldInUnion.GetValue(this);

    public IReadOnlyList<Stmt> Inits => _inits;

    public bool IsExplicit => !Handle.IsImplicit;

    public bool IsTransparent => Handle.IsTransparent;

    public uint NumInits => NumChildren;

    private static unsafe FieldDecl InitializedFieldInUnionFactory(InitListExpr self) => self.TranslationUnit.GetOrCreate<FieldDecl>(self.Handle.Referenced);

    private static unsafe Expr ArrayFillerFactory(InitListExpr self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.SubExpr);
}
