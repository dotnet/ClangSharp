// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConditionalOperator : AbstractConditionalOperator
    {
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<Expr> _rhs;

        internal ConditionalOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ConditionalOperator, CX_StmtClass.CX_StmtClass_ConditionalOperator)
        {
            _lhs = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.LhsExpr));
            _rhs = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.RhsExpr));
        }

        public Expr LHS => _lhs.Value;

        public Expr RHS => _rhs.Value;
    }
}
