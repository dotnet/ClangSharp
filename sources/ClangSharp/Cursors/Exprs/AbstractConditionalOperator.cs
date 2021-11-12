// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class AbstractConditionalOperator : Expr
    {
        private protected AbstractConditionalOperator(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastAbstractConditionalOperator or < CX_StmtClass.CX_StmtClass_FirstAbstractConditionalOperator)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }

        public Expr Cond => this is ConditionalOperator co ? co.Cond : ((BinaryConditionalOperator)this).Cond;

        public Expr FalseExpr => this is ConditionalOperator co ? co.FalseExpr : ((BinaryConditionalOperator)this).FalseExpr;

        public Expr TrueExpr => this is ConditionalOperator co ? co.TrueExpr : ((BinaryConditionalOperator)this).TrueExpr;
    }
}
