// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class WhileStmt : Stmt
    {
        private readonly Lazy<Expr> _cond;
        private readonly Lazy<Stmt> _body;

        internal WhileStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_WhileStmt, CX_StmtClass.CX_StmtClass_WhileStmt)
        {
            _cond = new Lazy<Expr>(() => Children.OfType<Expr>().ElementAt(0));
            _body = new Lazy<Stmt>(() => Children.OfType<Stmt>().ElementAt(1));
        }

        public Expr Cond => _cond.Value;

        public Stmt Body => _body.Value;
    }
}
