// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DependentSizedMatrixType : MatrixType
{
    private readonly ValueLazy<Expr> _rowExpr;
    private readonly ValueLazy<Expr> _columnExpr;

    internal DependentSizedMatrixType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_DependentSizedMatrix)
    {
        _rowExpr = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.RowExpr));
        _columnExpr = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.ColumnExpr));
    }

    public Expr ColumnExpr => _columnExpr.Value;

    public Expr RowExpr => _rowExpr.Value;
}
