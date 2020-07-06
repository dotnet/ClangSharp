// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SwitchStmt : Stmt
    {
        private readonly Lazy<Stmt> _body;
        private readonly Lazy<Expr> _cond;
        private readonly Lazy<VarDecl> _conditionVariable;
        private readonly Lazy<DeclStmt> _conditionVariableDeclStmt;
        private readonly Lazy<Stmt> _init;
        private readonly Lazy<SwitchCase> _switchCaseList;

        internal SwitchStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SwitchStmt, CX_StmtClass.CX_StmtClass_SwitchStmt)
        {
            _body = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.Body));
            _cond = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.CondExpr));
            _conditionVariable = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
            _conditionVariableDeclStmt = new Lazy<DeclStmt>(() => TranslationUnit.GetOrCreate<DeclStmt>(Handle.ConditionVariableDeclStmt));
            _init = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.InitExpr));
            _switchCaseList = new Lazy<SwitchCase>(() => TranslationUnit.GetOrCreate<SwitchCase>(Handle.SubStmt));
        }

        public Stmt Body => _body.Value;

        public Expr Cond => _cond.Value;

        public VarDecl ConditionVariable => _conditionVariable.Value;

        public DeclStmt ConditionVariableDeclStmt => _conditionVariableDeclStmt.Value;

        public Stmt Init => _init.Value;

        public SwitchCase SwitchCaseList => _switchCaseList.Value;
    }
}
