// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCAtSynchronizedStmt : Stmt
    {
        internal ObjCAtSynchronizedStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAtSynchronizedStmt, CX_StmtClass.CX_StmtClass_ObjCAtSynchronizedStmt)
        {
            Debug.Assert(NumChildren is 2);
        }

        public CompoundStmt SynchBody => (CompoundStmt)Children[1];

        public Expr SynchExpr => (Expr)Children[0];
    }
}
