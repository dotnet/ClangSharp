// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ForStmt : Stmt
    {
        internal ForStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ForStmt, CX_StmtClass.CX_StmtClass_ForStmt)
        {
            Debug.Assert(NumChildren is 5);
        }

        public Stmt Body => Children[4];

        public Expr Cond => (Expr)Children[2];

        public VarDecl ConditionVariable => (VarDecl)ConditionVariableDeclStmt?.SingleDecl;

        public DeclStmt ConditionVariableDeclStmt => (DeclStmt)Children[1];

        public Expr Inc => (Expr)Children[3];

        public Stmt Init => Children[0];
    }
}
