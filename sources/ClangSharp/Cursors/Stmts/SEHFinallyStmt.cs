// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SEHFinallyStmt : Stmt
    {
        internal SEHFinallyStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SEHFinallyStmt, CX_StmtClass.CX_StmtClass_SEHFinallyStmt)
        {
            Debug.Assert(NumChildren is 1);
        }

        public CompoundStmt Block => (CompoundStmt)Children[0];
    }
}
