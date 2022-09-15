// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public class BinaryOperator : Expr
{
    internal BinaryOperator(CXCursor handle) : this(handle, CXCursorKind.CXCursor_BinaryOperator, CX_StmtClass.CX_StmtClass_BinaryOperator)
    {
    }

    private protected BinaryOperator(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastBinaryOperator or < CX_StmtClass.CX_StmtClass_FirstBinaryOperator)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        Debug.Assert(NumChildren is 2);
    }

    public bool IsAdditiveOp => Opcode is CX_BinaryOperatorKind.CX_BO_Add or CX_BinaryOperatorKind.CX_BO_Sub;

    public bool IsAssignmentOp => Opcode is >= CX_BinaryOperatorKind.CX_BO_Assign and <= CX_BinaryOperatorKind.CX_BO_OrAssign;

    public bool IsBitwiseOp => Opcode is >= CX_BinaryOperatorKind.CX_BO_And and <= CX_BinaryOperatorKind.CX_BO_Or;

    public bool IsCommaOp => Opcode == CX_BinaryOperatorKind.CX_BO_Comma;

    public bool IsComparisonOp => Opcode is >= CX_BinaryOperatorKind.CX_BO_Cmp and <= CX_BinaryOperatorKind.CX_BO_NE;

    public bool IsCompoundAssignmentOp=> Opcode is > CX_BinaryOperatorKind.CX_BO_Assign and <= CX_BinaryOperatorKind.CX_BO_OrAssign;

    public bool IsEqualityOp => Opcode is CX_BinaryOperatorKind.CX_BO_EQ or CX_BinaryOperatorKind.CX_BO_NE;

    public bool IsLogicalOp => Opcode is CX_BinaryOperatorKind.CX_BO_LAnd or CX_BinaryOperatorKind.CX_BO_LOr;

    public bool IsMultiplicativeOp => Opcode is >= CX_BinaryOperatorKind.CX_BO_Mul and <= CX_BinaryOperatorKind.CX_BO_Rem;

    public bool IsPtrMemOp => Opcode is CX_BinaryOperatorKind.CX_BO_PtrMemD or CX_BinaryOperatorKind.CX_BO_PtrMemI;

    public bool IsRelationalOp => Opcode is >= CX_BinaryOperatorKind.CX_BO_LT and <= CX_BinaryOperatorKind.CX_BO_GE;

    public bool IsShiftAssignOp=> Opcode is CX_BinaryOperatorKind.CX_BO_ShlAssign or CX_BinaryOperatorKind.CX_BO_ShrAssign;

    public bool IsShiftOp => Opcode is CX_BinaryOperatorKind.CX_BO_Shl or CX_BinaryOperatorKind.CX_BO_Shr;

    public Expr LHS => (Expr)Children[0];

    public CX_BinaryOperatorKind Opcode => Handle.BinaryOperatorKind;

    public string OpcodeStr => Handle.BinaryOperatorKindSpelling.CString;

    public Expr RHS => (Expr)Children[1];
}
