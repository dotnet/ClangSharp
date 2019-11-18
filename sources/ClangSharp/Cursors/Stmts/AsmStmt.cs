// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class AsmStmt : Stmt
    {
        private protected AsmStmt(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastAsmStmt < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstAsmStmt))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
