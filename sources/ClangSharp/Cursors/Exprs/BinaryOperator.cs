// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class BinaryOperator : Expr
    {
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<Expr> _rhs;

        internal BinaryOperator(CXCursor handle) : this(handle, CXCursorKind.CXCursor_BinaryOperator, CX_StmtClass.CX_StmtClass_BinaryOperator)
        {
        }

        private protected BinaryOperator(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastBinaryOperator < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstBinaryOperator))
            {
                throw new ArgumentException(nameof(handle));
            }
            Debug.Assert(Children.OfType<Expr>().Count() == 2);

            _lhs = new Lazy<Expr>(() => Children.OfType<Expr>().First());
            _rhs = new Lazy<Expr>(() => Children.OfType<Expr>().Last());
        }

        public Expr LHS => _lhs.Value;

        public CX_BinaryOperatorKind Opcode => Handle.BinaryOperatorKind;

        public string OpcodeStr => Handle.BinaryOperatorKindSpelling.ToString();

        public Expr RHS => _rhs.Value;
    }
}
