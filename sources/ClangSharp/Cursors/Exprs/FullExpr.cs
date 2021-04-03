// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FullExpr : Expr
    {
        private protected FullExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastFullExpr or < CX_StmtClass.CX_StmtClass_FirstFullExpr)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            Debug.Assert(NumChildren is 1);
        }

        public Expr SubExpr => (Expr)Children[0];
    }
}
