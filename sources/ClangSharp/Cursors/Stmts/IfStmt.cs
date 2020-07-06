// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IfStmt : Stmt
    {
        private readonly Lazy<Expr> _cond;
        private readonly Lazy<VarDecl> _conditionVariable;
        private readonly Lazy<DeclStmt> _conditionVariableDeclStmt;
        private readonly Lazy<Stmt> _else;
        private readonly Lazy<Stmt> _then;

        internal IfStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_IfStmt, CX_StmtClass.CX_StmtClass_IfStmt)
        {
            _cond = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.CondExpr));
            _conditionVariable = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
            _conditionVariableDeclStmt = new Lazy<DeclStmt>(() => TranslationUnit.GetOrCreate<DeclStmt>(Handle.ConditionVariableDeclStmt));
            _else = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.SubStmt));
            _then = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.Body));
        }

        public Expr Cond => _cond.Value;

        public VarDecl ConditionVariable => _conditionVariable.Value;

        public DeclStmt ConditionVariableDeclStmt => _conditionVariableDeclStmt.Value;

        public Stmt Else => _else.Value;

        public Stmt Then => _then.Value;
    }
}
