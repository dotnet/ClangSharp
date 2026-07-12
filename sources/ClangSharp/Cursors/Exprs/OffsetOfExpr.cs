// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class OffsetOfExpr : Expr
{
    private readonly LazyList<Expr, Stmt> _indexExprs;
    private ValueLazy<OffsetOfExpr, Cursor?> _referenced;
    private ValueLazy<OffsetOfExpr, Type> _typeSourceInfoType;

    internal unsafe OffsetOfExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_OffsetOfExpr)
    {
        _indexExprs = LazyList.Create<Expr, Stmt>(_children);
        _referenced = new ValueLazy<OffsetOfExpr, Cursor?>(&ReferencedFactory);
        _typeSourceInfoType = new ValueLazy<OffsetOfExpr, Type>(&TypeSourceInfoTypeFactory);
    }

    public IReadOnlyList<Expr> IndexExprs => _indexExprs;

    public uint NumExpressions => NumChildren;

    public Cursor? Referenced => _referenced.GetValue(this);

    public Type TypeSourceInfoType => _typeSourceInfoType.GetValue(this);

    private static unsafe Type TypeSourceInfoTypeFactory(OffsetOfExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);

    private static unsafe Cursor? ReferencedFactory(OffsetOfExpr self) => !self.Handle.Referenced.IsNull ? self.TranslationUnit.GetOrCreate<Cursor>(self.Handle.Referenced) : null;
}
