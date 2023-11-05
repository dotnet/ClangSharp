// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXUnaryOperatorKind;

namespace ClangSharp;

public sealed class UnaryOperator : Expr
{
    internal UnaryOperator(CXCursor handle) : base(handle, CXCursor_UnaryOperator, CX_StmtClass_UnaryOperator)
    {
        Debug.Assert(NumChildren is 1);
    }

    public bool IsArithmetic => Opcode is >= CXUnaryOperator_Plus and <= CXUnaryOperator_LNot;

    public bool IsDecrementOp => Opcode is CXUnaryOperator_PreDec or CXUnaryOperator_PostDec;

    public bool IsIncrementOp => Opcode is CXUnaryOperator_PreInc or CXUnaryOperator_PostInc;

    public bool IsIncrementDecrementOp => Opcode <= CXUnaryOperator_PreDec;

    public bool IsPrefix => Opcode is CXUnaryOperator_PreInc or CXUnaryOperator_PreDec;

    public bool IsPostfix => Opcode is CXUnaryOperator_PostInc or CXUnaryOperator_PostDec;

    public CXUnaryOperatorKind Opcode => Handle.UnaryOperatorKind;

    public string OpcodeStr => Handle.UnaryOperatorKindSpelling.ToString();

    public Expr SubExpr => (Expr)Children[0];
}
