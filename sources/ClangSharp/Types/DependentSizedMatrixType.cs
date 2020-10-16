// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
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
}
