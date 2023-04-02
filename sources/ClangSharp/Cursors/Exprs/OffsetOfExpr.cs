// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class OffsetOfExpr : Expr
{
    private readonly Lazy<IReadOnlyList<Expr>> _indexExprs;
    private readonly Lazy<Cursor?> _referenced;
    private readonly Lazy<Type> _typeSourceInfoType;

    internal OffsetOfExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_OffsetOfExpr)
    {
        _indexExprs = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
        _referenced = new Lazy<Cursor?>(() => !Handle.Referenced.IsNull ? TranslationUnit.GetOrCreate<Cursor>(Handle.Referenced) : null);
        _typeSourceInfoType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public IReadOnlyList<Expr> IndexExprs => _indexExprs.Value;

    public uint NumExpressions => NumChildren;

    public Cursor? Referenced => _referenced.Value;

    public Type TypeSourceInfoType => _typeSourceInfoType.Value;
}
