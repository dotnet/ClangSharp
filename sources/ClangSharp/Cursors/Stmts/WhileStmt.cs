// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class WhileStmt : Stmt
    {
        internal WhileStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_WhileStmt, CX_StmtClass.CX_StmtClass_WhileStmt)
        {
            Debug.Assert(NumChildren is 2 or 3);
        }

        public Stmt Body => Children[BodyOffset];

        public Expr Cond => (Expr)Children[CondOffset];

        public bool HasVarStorage => Handle.HasVarStorage;

        public VarDecl ConditionVariable => (VarDecl)ConditionVariableDeclStmt?.SingleDecl;

        public DeclStmt ConditionVariableDeclStmt => HasVarStorage ? (DeclStmt)Children[VarOffset] : null;

        private int BodyOffset => CondOffset + 1;

        private int CondOffset => VarOffset + (HasVarStorage ? 1 : 0);

        private static int VarOffset => 0;
    }
}
