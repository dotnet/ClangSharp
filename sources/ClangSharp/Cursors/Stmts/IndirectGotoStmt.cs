// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IndirectGotoStmt : Stmt
    {
        private readonly Lazy<LabelDecl> _constantTarget;
        private readonly Lazy<Expr> _target;

        internal IndirectGotoStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_IndirectGotoStmt, CX_StmtClass.CX_StmtClass_IndirectGotoStmt)
        {
            _constantTarget = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
            _target = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubStmt));
        }

        public LabelDecl ConstantTarget => _constantTarget.Value;

        public Expr Target => _target.Value;
    }
}
