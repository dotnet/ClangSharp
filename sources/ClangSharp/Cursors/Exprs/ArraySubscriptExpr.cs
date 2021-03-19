// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArraySubscriptExpr : Expr
    {
        internal ArraySubscriptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ArraySubscriptExpr, CX_StmtClass.CX_StmtClass_ArraySubscriptExpr)
        {
            Debug.Assert(NumChildren is 2);
        }

        public Expr Base => LHSIsBase ? LHS : RHS;

        public Expr Idx => LHSIsBase ? RHS : LHS;

        public Expr LHS => (Expr)Children[0];

        public Expr RHS => (Expr)Children[1];

        private bool LHSIsBase => RHS.Type.IsIntegerType;
    }
}
