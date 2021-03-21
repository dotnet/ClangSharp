// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class WhileStmt : Stmt
    {
        private readonly Lazy<Stmt> _body;
        private readonly Lazy<Expr> _cond;
        private readonly Lazy<VarDecl> _conditionVariable;
        private readonly Lazy<DeclStmt> _conditionVariableDeclStmt;

        internal WhileStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_WhileStmt, CX_StmtClass.CX_StmtClass_WhileStmt)
        {
            _body = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.Body));
            _cond = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.CondExpr));
            _conditionVariable = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
            _conditionVariableDeclStmt = new Lazy<DeclStmt>(() => TranslationUnit.GetOrCreate<DeclStmt>(Handle.ConditionVariableDeclStmt));
        }

        public Stmt Body => _body.Value;

        public Expr Cond => _cond.Value;

        public VarDecl ConditionVariable => _conditionVariable.Value;

        public DeclStmt ConditionVariableDeclStmt => _conditionVariableDeclStmt.Value;
    }
}
