// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MatrixSubscriptExpr : Expr
    {
        private readonly Lazy<Expr> _base;
        private readonly Lazy<Expr> _rowIdx;
        private readonly Lazy<Expr> _columnIdx;

        internal MatrixSubscriptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_MatrixSubscriptExpr)
        {
            _base = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.BaseExpr));
            _rowIdx = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.RowIdxExpr));
            _columnIdx = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.ColumnIdxExpr));
        }

        public Expr Base => _base.Value;

        public Expr ColumnIdx => _columnIdx.Value;

        public bool IsIncomplete => Handle.IsIncomplete;

        public Expr RowIdx => _rowIdx.Value;
    }
}
