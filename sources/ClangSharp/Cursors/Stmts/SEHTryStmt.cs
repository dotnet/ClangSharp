// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SEHTryStmt : Stmt
    {
        internal SEHTryStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SEHTryStmt, CX_StmtClass.CX_StmtClass_SEHTryStmt)
        {
            Debug.Assert(NumChildren is 2);
        }

        public SEHExceptStmt ExceptHandler => Handler as SEHExceptStmt;

        public SEHFinallyStmt FinallyHandler => Handler as SEHFinallyStmt;

        public bool IsCXXTry => Handle.IsCXXTry;

        public Stmt Handler => Children[1];

        public CompoundStmt TryBlock => (CompoundStmt)Children[0];
    }
}
