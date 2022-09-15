// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class UnaryOperator : Expr
{
    internal UnaryOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryOperator, CX_StmtClass.CX_StmtClass_UnaryOperator)
    {
        Debug.Assert(NumChildren is 1);
    }

    public bool IsArithmetic => Opcode is >= CX_UnaryOperatorKind.CX_UO_Plus and <= CX_UnaryOperatorKind.CX_UO_LNot;

    public bool IsDecrementOp => Opcode is CX_UnaryOperatorKind.CX_UO_PreDec or CX_UnaryOperatorKind.CX_UO_PostDec;

    public bool IsIncrementOp => Opcode is CX_UnaryOperatorKind.CX_UO_PreInc or CX_UnaryOperatorKind.CX_UO_PostInc;

    public bool IsIncrementDecrementOp => Opcode <= CX_UnaryOperatorKind.CX_UO_PreDec;

    public bool IsPrefix => Opcode is CX_UnaryOperatorKind.CX_UO_PreInc or CX_UnaryOperatorKind.CX_UO_PreDec;

    public bool IsPostfix => Opcode is CX_UnaryOperatorKind.CX_UO_PostInc or CX_UnaryOperatorKind.CX_UO_PostDec;

    public CX_UnaryOperatorKind Opcode => Handle.UnaryOperatorKind;

    public string OpcodeStr => Handle.UnaryOperatorKindSpelling.ToString();

    public Expr SubExpr => (Expr)Children[0];
}
