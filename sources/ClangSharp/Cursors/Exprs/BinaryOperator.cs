// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class BinaryOperator : Expr
    {
        internal BinaryOperator(CXCursor handle) : this(handle, CXCursorKind.CXCursor_BinaryOperator, CX_StmtClass.CX_StmtClass_BinaryOperator)
        {
        }

        private protected BinaryOperator(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastBinaryOperator < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstBinaryOperator))
            {
                throw new ArgumentException(nameof(handle));
            }

            Debug.Assert(NumChildren is 2);
        }

        public bool IsAdditiveOp => (Opcode == CX_BinaryOperatorKind.CX_BO_Add) || (Opcode == CX_BinaryOperatorKind.CX_BO_Sub);

        public bool IsAssignmentOp => (Opcode >= CX_BinaryOperatorKind.CX_BO_Assign) && (Opcode <= CX_BinaryOperatorKind.CX_BO_OrAssign);

        public bool IsBitwiseOp => (Opcode >= CX_BinaryOperatorKind.CX_BO_And) && (Opcode <= CX_BinaryOperatorKind.CX_BO_Or);

        public bool IsCommaOp => Opcode == CX_BinaryOperatorKind.CX_BO_Comma;

        public bool IsComparisonOp => (Opcode >= CX_BinaryOperatorKind.CX_BO_Cmp) && (Opcode <= CX_BinaryOperatorKind.CX_BO_NE);

        public bool IsCompoundAssignmentOp=> (Opcode > CX_BinaryOperatorKind.CX_BO_Assign) && (Opcode <= CX_BinaryOperatorKind.CX_BO_OrAssign);

        public bool IsEqualityOp => (Opcode == CX_BinaryOperatorKind.CX_BO_EQ) || (Opcode == CX_BinaryOperatorKind.CX_BO_NE);

        public bool IsLogicalOp => (Opcode == CX_BinaryOperatorKind.CX_BO_LAnd) || (Opcode == CX_BinaryOperatorKind.CX_BO_LOr);

        public bool IsMultiplicativeOp => (Opcode >= CX_BinaryOperatorKind.CX_BO_Mul) && (Opcode <= CX_BinaryOperatorKind.CX_BO_Rem);

        public bool IsPtrMemOp => (Opcode == CX_BinaryOperatorKind.CX_BO_PtrMemD) || (Opcode == CX_BinaryOperatorKind.CX_BO_PtrMemI);

        public bool IsRelationalOp => (Opcode >= CX_BinaryOperatorKind.CX_BO_LT) && (Opcode <= CX_BinaryOperatorKind.CX_BO_GE);

        public bool IsShiftAssignOp=> (Opcode == CX_BinaryOperatorKind.CX_BO_ShlAssign) || (Opcode == CX_BinaryOperatorKind.CX_BO_ShrAssign);

        public bool IsShiftOp => (Opcode == CX_BinaryOperatorKind.CX_BO_Shl) || (Opcode == CX_BinaryOperatorKind.CX_BO_Shr);

        public Expr LHS => (Expr)Children[0];

        public CX_BinaryOperatorKind Opcode => Handle.BinaryOperatorKind;

        public string OpcodeStr => Handle.BinaryOperatorKindSpelling.CString;

        public Expr RHS => (Expr)Children[1];
    }
}
