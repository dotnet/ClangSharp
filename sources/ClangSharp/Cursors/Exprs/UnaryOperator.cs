// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryOperator : Expr
    {
        internal UnaryOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryOperator, CX_StmtClass.CX_StmtClass_UnaryOperator)
        {
            Debug.Assert(NumChildren is 1);
        }

        public bool IsArithmetic => (Opcode >= CX_UnaryOperatorKind.CX_UO_Plus) && (Opcode <= CX_UnaryOperatorKind.CX_UO_LNot);

        public bool IsDecrementOp => (Opcode == CX_UnaryOperatorKind.CX_UO_PreDec) || (Opcode == CX_UnaryOperatorKind.CX_UO_PostDec);

        public bool IsIncrementOp => (Opcode == CX_UnaryOperatorKind.CX_UO_PreInc) || (Opcode == CX_UnaryOperatorKind.CX_UO_PostInc);

        public bool IsIncrementDecrementOp => (Opcode <= CX_UnaryOperatorKind.CX_UO_PreDec);

        public bool IsPrefix => (Opcode == CX_UnaryOperatorKind.CX_UO_PreInc) || (Opcode == CX_UnaryOperatorKind.CX_UO_PreDec);

        public bool IsPostfix => (Opcode == CX_UnaryOperatorKind.CX_UO_PostInc) || (Opcode == CX_UnaryOperatorKind.CX_UO_PostDec);

        public CX_UnaryOperatorKind Opcode => Handle.UnaryOperatorKind;

        public string OpcodeStr => Handle.UnaryOperatorKindSpelling.ToString();

        public Expr SubExpr => (Expr)Children[0];
    }
}
