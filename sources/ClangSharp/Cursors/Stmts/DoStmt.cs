// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DoStmt : Stmt
    {
        internal DoStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DoStmt, CX_StmtClass.CX_StmtClass_DoStmt)
        {
            Debug.Assert(NumChildren is 2);
        }

        public Stmt Body => Children[0];

        public Expr Cond => (Expr)Children[1];
    }
}
