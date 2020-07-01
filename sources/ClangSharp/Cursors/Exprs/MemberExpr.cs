// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MemberExpr : Expr
    {
        private readonly Lazy<Expr> _base;
        private readonly Lazy<ValueDecl> _memberDecl;

        internal MemberExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MemberRefExpr, CX_StmtClass.CX_StmtClass_MemberExpr)
        {
            _base = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
            _memberDecl = new Lazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.Referenced));
        }

        public Expr Base => _base.Value;

        public ValueDecl MemberDecl => _memberDecl.Value;
    }
}
