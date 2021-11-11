// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class OMPLoopBasedDirective : OMPExecutableDirective
    {
        private protected OMPLoopBasedDirective(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastOMPLoopBasedDirective or < CX_StmtClass.CX_StmtClass_FirstOMPLoopBasedDirective)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }
    }
}
