// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class DependentSizedMatrixType : MatrixType
{
    private readonly Lazy<Expr> _rowExpr;
    private readonly Lazy<Expr> _columnExpr;

    internal DependentSizedMatrixType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_DependentSizedMatrix)
    {
        _rowExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.RowExpr));
        _columnExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.ColumnExpr));
    }

    public Expr ColumnExpr => _columnExpr.Value;

    public Expr RowExpr => _rowExpr.Value;
}
