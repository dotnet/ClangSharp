// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXRewrittenBinaryOperator : Expr
    {
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<Expr> _rhs;

        internal CXXRewrittenBinaryOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_CXXRewrittenBinaryOperator)
        {
            Debug.Assert(NumChildren is 1);

            _lhs = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.LhsExpr));
            _rhs = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.RhsExpr));
        }

        public Expr LHS => _lhs.Value;

        public static bool IsAssignmentOp => false;

        public static bool IsComparisonOp => true;

        public CX_BinaryOperatorKind Opcode => Operator;

        public CX_BinaryOperatorKind Operator => Handle.BinaryOperatorKind;

        public string OpcodeStr => Handle.BinaryOperatorKindSpelling.CString;

        public Expr RHS => _rhs.Value;

        public Expr SemanticForm => (Expr)Children[0];
    }
}
