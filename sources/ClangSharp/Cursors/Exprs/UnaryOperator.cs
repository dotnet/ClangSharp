// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryOperator : Expr
    {
        private readonly Lazy<Expr> _subExpr;

        internal UnaryOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryOperator, CX_StmtClass.CX_StmtClass_UnaryOperator)
        {
            _subExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
        }

        public bool IsPrefix => (Opcode == CX_UnaryOperatorKind.CX_UO_PreInc) || (Opcode == CX_UnaryOperatorKind.CX_UO_PreDec);

        public bool IsPostfix => (Opcode == CX_UnaryOperatorKind.CX_UO_PostInc) || (Opcode == CX_UnaryOperatorKind.CX_UO_PostDec);

        public CX_UnaryOperatorKind Opcode => Handle.UnaryOperatorKind;

        public string OpcodeStr => Handle.UnaryOperatorKindSpelling.ToString();

        public Expr SubExpr => _subExpr.Value;
    }
}
