// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCIsaExpr : Expr
    {
        internal ObjCIsaExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MemberRefExpr, CX_StmtClass.CX_StmtClass_ObjCIsaExpr)
        {
            Debug.Assert(NumChildren is 1);
        }

        public Expr Base => (Expr)Children[0];

        public bool IsArrow => Handle.IsArrow;
    }
}
