// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConditionalOperator : AbstractConditionalOperator
    {
        internal ConditionalOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ConditionalOperator, CX_StmtClass.CX_StmtClass_ConditionalOperator)
        {
            Debug.Assert(NumChildren is 3);
        }

        public new Expr Cond => (Expr)Children[0];

        public new Expr FalseExpr => RHS;

        public Expr LHS => (Expr)Children[1];

        public Expr RHS => (Expr)Children[2];

        public new Expr TrueExpr => LHS;
    }
}
