// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXFoldExpr : Expr
    {
        internal CXXFoldExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_CXXFoldExpr)
        {
            Debug.Assert(NumChildren is 3);
        }

        public UnresolvedLookupExpr Callee => (UnresolvedLookupExpr)Children[0];

        public Expr Init => IsLeftFold ? LHS : RHS;

        public bool IsLeftFold => !IsRightFold;

        public bool IsRightFold => (LHS != null) && LHS.ContainsUnexpandedParameterPack;

        public Expr LHS => (Expr)Children[1];

        public CX_BinaryOperatorKind Operator => Handle.BinaryOperatorKind;

        public Expr Pattern => IsLeftFold ? RHS : LHS;

        public Expr RHS => (Expr)Children[2];
    }
}
