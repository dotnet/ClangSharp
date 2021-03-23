// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCForCollectionStmt : Stmt
    {
        internal ObjCForCollectionStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCForCollectionStmt, CX_StmtClass.CX_StmtClass_ObjCForCollectionStmt)
        {
            Debug.Assert(NumChildren is 3);
        }

        public Stmt Body => Children[2];

        public Expr Collection => (Expr)Children[1];

        public Stmt Element => Children[0];
    }
}
