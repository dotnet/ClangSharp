// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ChooseExpr : Expr
    {
        internal ChooseExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ChooseExpr)
        {
            Debug.Assert(NumChildren is 3);
        }

        public Expr ChosenSubExpr => IsConditionDependent ? (IsConditionTrue ? LHS : RHS) : null;

        public Expr Cond => (Expr)Children[0];

        public bool IsConditionTrue => Handle.IsConditionTrue;

        public bool IsConditionDependent => Cond.IsTypeDependent || Cond.IsValueDependent;

        public Expr LHS => (Expr)Children[1];

        public Expr RHS => (Expr)Children[2];
    }
}
