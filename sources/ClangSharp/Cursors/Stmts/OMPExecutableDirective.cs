// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class OMPExecutableDirective : Stmt
    {
        private protected OMPExecutableDirective(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastOMPExecutableDirective or < CX_StmtClass.CX_StmtClass_FirstOMPExecutableDirective)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }
    }
}
