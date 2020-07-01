// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArraySubscriptExpr : Expr
    {
        private readonly Lazy<Expr> _base;
        private readonly Lazy<Expr> _idx;
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<Expr> _rhs;

        internal ArraySubscriptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ArraySubscriptExpr, CX_StmtClass.CX_StmtClass_ArraySubscriptExpr)
        {
            _base = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.BaseExpr));
            _idx = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.IdxExpr));
            _lhs = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.LhsExpr));
            _rhs = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.RhsExpr));
        }

        public Expr Base => _base.Value;

        public Expr Idx => _idx.Value;

        public Expr LHS => _lhs.Value;

        public Expr RHS => _rhs.Value;
    }
}
