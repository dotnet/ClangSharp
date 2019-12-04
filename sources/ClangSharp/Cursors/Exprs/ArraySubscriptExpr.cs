// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArraySubscriptExpr : Expr
    {
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<Expr> _rhs;

        internal ArraySubscriptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ArraySubscriptExpr, CX_StmtClass.CX_StmtClass_ArraySubscriptExpr)
        {
            _lhs = new Lazy<Expr>(() => Children.OfType<Expr>().ElementAt(0));
            _rhs = new Lazy<Expr>(() => Children.OfType<Expr>().ElementAt(1));
        }

        public Expr Base => RHS.Type.IsIntegerType ? LHS : RHS;

        public Expr Idx => RHS.Type.IsIntegerType ? RHS : LHS;

        public Expr LHS => _lhs.Value;

        public Expr RHS => _rhs.Value;
    }
}
