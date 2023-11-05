// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CXBinaryOperatorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class BinaryOperator : Expr
{
    internal BinaryOperator(CXCursor handle) : this(handle, CXCursor_BinaryOperator, CX_StmtClass_BinaryOperator)
    {
    }

    private protected BinaryOperator(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastBinaryOperator or < CX_StmtClass_FirstBinaryOperator)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        Debug.Assert(NumChildren is 2);
    }

    public bool IsAdditiveOp => Opcode is CXBinaryOperator_Add or CXBinaryOperator_Sub;

    public bool IsAssignmentOp => Opcode is >= CXBinaryOperator_Assign and <= CXBinaryOperator_OrAssign;

    public bool IsBitwiseOp => Opcode is >= CXBinaryOperator_And and <= CXBinaryOperator_Or;

    public bool IsCommaOp => Opcode == CXBinaryOperator_Comma;

    public bool IsComparisonOp => Opcode is >= CXBinaryOperator_Cmp and <= CXBinaryOperator_NE;

    public bool IsCompoundAssignmentOp=> Opcode is > CXBinaryOperator_Assign and <= CXBinaryOperator_OrAssign;

    public bool IsEqualityOp => Opcode is CXBinaryOperator_EQ or CXBinaryOperator_NE;

    public bool IsLogicalOp => Opcode is CXBinaryOperator_LAnd or CXBinaryOperator_LOr;

    public bool IsMultiplicativeOp => Opcode is >= CXBinaryOperator_Mul and <= CXBinaryOperator_Rem;

    public bool IsPtrMemOp => Opcode is CXBinaryOperator_PtrMemD or CXBinaryOperator_PtrMemI;

    public bool IsRelationalOp => Opcode is >= CXBinaryOperator_LT and <= CXBinaryOperator_GE;

    public bool IsShiftAssignOp=> Opcode is CXBinaryOperator_ShlAssign or CXBinaryOperator_ShrAssign;

    public bool IsShiftOp => Opcode is CXBinaryOperator_Shl or CXBinaryOperator_Shr;

    public Expr LHS => (Expr)Children[0];

    public CXBinaryOperatorKind Opcode => Handle.BinaryOperatorKind;

    public string OpcodeStr => Handle.BinaryOperatorKindSpelling.CString;

    public Expr RHS => (Expr)Children[1];
}
