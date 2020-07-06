// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXFoldExpr : Expr
    {
        private readonly Lazy<Expr> _init;
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<Expr> _pattern;
        private readonly Lazy<Expr> _rhs;

        internal CXXFoldExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_CXXFoldExpr)
        {
            _init = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.InitExpr));
            _lhs = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.LhsExpr));
            _pattern = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
            _rhs = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.RhsExpr));
        }

        public Expr Init => _init.Value;

        public Expr LHS => _lhs.Value;

        public CX_BinaryOperatorKind Operator => Handle.BinaryOperatorKind;

        public Expr Pattern => _pattern.Value;

        public Expr RHS => _rhs.Value;
    }
}
