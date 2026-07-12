// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DependentSizedMatrixType : MatrixType
{
    private ValueLazy<DependentSizedMatrixType, Expr> _rowExpr;
    private ValueLazy<DependentSizedMatrixType, Expr> _columnExpr;

    internal unsafe DependentSizedMatrixType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_DependentSizedMatrix)
    {
        _rowExpr = new ValueLazy<DependentSizedMatrixType, Expr>(&RowExprFactory);
        _columnExpr = new ValueLazy<DependentSizedMatrixType, Expr>(&ColumnExprFactory);
    }

    public Expr ColumnExpr => _columnExpr.GetValue(this);

    public Expr RowExpr => _rowExpr.GetValue(this);

    private static unsafe Expr ColumnExprFactory(DependentSizedMatrixType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.ColumnExpr);

    private static unsafe Expr RowExprFactory(DependentSizedMatrixType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.RowExpr);
}
