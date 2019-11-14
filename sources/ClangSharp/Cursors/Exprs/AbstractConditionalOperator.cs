// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class AbstractConditionalOperator : Expr
    {
        private protected AbstractConditionalOperator(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastAbstractConditionalOperator < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstAbstractConditionalOperator))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
