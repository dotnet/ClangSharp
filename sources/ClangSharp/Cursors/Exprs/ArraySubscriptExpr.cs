// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
