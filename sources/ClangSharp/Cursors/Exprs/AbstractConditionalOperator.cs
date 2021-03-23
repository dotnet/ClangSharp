// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class AbstractConditionalOperator : Expr
    {
        private protected AbstractConditionalOperator(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastAbstractConditionalOperator < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstAbstractConditionalOperator))
            {
                throw new ArgumentException(nameof(handle));
            }
        }

        public Expr Cond
        {
            get
            {
                if (this is ConditionalOperator co)
                {
                    return co.Cond;
                }
                return ((BinaryConditionalOperator)this).Cond;
            }
        }

        public Expr FalseExpr
        {
            get
            {
                if (this is ConditionalOperator co)
                {
                    return co.FalseExpr;
                }
                return ((BinaryConditionalOperator)this).FalseExpr;
            }
        }

        public Expr TrueExpr
        {
            get
            {
                if (this is ConditionalOperator co)
                {
                    return co.TrueExpr;
                }
                return ((BinaryConditionalOperator)this).TrueExpr;
            }
        }
    }
}
