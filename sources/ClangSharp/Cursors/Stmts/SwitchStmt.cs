// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SwitchStmt : Stmt
    {
        private readonly Lazy<SwitchCase> _switchCaseList;

        internal SwitchStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SwitchStmt, CX_StmtClass.CX_StmtClass_SwitchStmt)
        {
            Debug.Assert(NumChildren is >= 2 and <= 4);
            _switchCaseList = new Lazy<SwitchCase>(() => TranslationUnit.GetOrCreate<SwitchCase>(Handle.SubStmt));
        }

        public Stmt Body => Children[BodyOffset];

        public Expr Cond => (Expr)Children[CondOffset];

        public VarDecl ConditionVariable => (VarDecl)ConditionVariableDeclStmt?.SingleDecl;

        public DeclStmt ConditionVariableDeclStmt => HasVarStorage ? (DeclStmt)Children[VarOffset] : null;

        public bool HasInitStorage => Handle.HasInit;

        public bool HasVarStorage => Handle.HasVarStorage;

        public Stmt Init => HasInitStorage ? Children[InitOffset] : null;

        public bool IsAllEnumCasesCovered => Handle.IsAllEnumCasesCovered;

        public SwitchCase SwitchCaseList => _switchCaseList.Value;

        private int BodyOffset => CondOffset + 1;

        private int CondOffset => VarOffset + (HasVarStorage ? 1 : 0);

        private int InitOffset => 0;

        private int VarOffset => InitOffset + (HasInitStorage ? 1 : 0);

    }
}
