// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IfStmt : Stmt
    {
        internal IfStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_IfStmt, CX_StmtClass.CX_StmtClass_IfStmt)
        {
            Debug.Assert(NumChildren is >= 2 and <= 4);
        }

        public Expr Cond => (Expr)Children[CondOffset];

        public VarDecl ConditionVariable => (VarDecl)ConditionVariableDeclStmt?.SingleDecl;

        public DeclStmt ConditionVariableDeclStmt => HasVarStorage ? (DeclStmt)Children[VarOffset] : null;

        public Stmt Else => HasElseStorage ? Children[ElseOffset] : null;

        public bool HasElseStorage => Handle.HasElseStorage;

        public bool HasInitStorage => Handle.HasInitStorage;

        public bool HasVarStorage => Handle.HasVarStorage;

        public Stmt Init => HasInitStorage ? Children[InitOffset] : null;

        public bool IsConstexpr => Handle.IsConstexpr;

        public bool IsObjcAvailabilityCheck => Cond is ObjCAvailabilityCheckExpr;

        public Stmt Then => Children[ThenOffset];

        private int CondOffset => VarOffset + (HasVarStorage ? 1 : 0);

        private int ElseOffset => CondOffset + 2;

        private static int InitOffset => 0;

        private int ThenOffset => CondOffset + 1;

        private int VarOffset => InitOffset + (HasInitStorage ? 1 : 0);
    }
}
