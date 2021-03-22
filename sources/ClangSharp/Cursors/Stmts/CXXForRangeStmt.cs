// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXForRangeStmt : Stmt
    {
        internal CXXForRangeStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXForRangeStmt, CX_StmtClass.CX_StmtClass_CXXForRangeStmt)
        {
            Debug.Assert(NumChildren is 8);
        }

        public DeclStmt BeginStmt => (DeclStmt)Children[2];

        public Stmt Body => Children[7];

        public Expr Cond => (Expr)Children[4];

        public DeclStmt EndStmt => (DeclStmt)Children[3];

        public Expr Inc => (Expr)Children[5];

        public VarDecl LoopVariable => (VarDecl)LoopVarStmt.SingleDecl;

        public DeclStmt LoopVarStmt => (DeclStmt)Children[6];

        public Stmt Init => Children[0];

        public Expr RangeInit => ((VarDecl)RangeStmt.SingleDecl).Init;

        public DeclStmt RangeStmt => (DeclStmt)Children[1];
    }
}
