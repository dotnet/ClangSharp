// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DoStmt : Stmt
    {
        private readonly Lazy<Stmt> _body;
        private readonly Lazy<Expr> _cond;

        internal DoStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DoStmt, CX_StmtClass.CX_StmtClass_DoStmt)
        {
            _body = new Lazy<Stmt>(() => Children.OfType<Stmt>().ElementAt(0));
            _cond = new Lazy<Expr>(() => Children.OfType<Expr>().ElementAt(Body is Expr ? 1 : 0));
        }

        public Stmt Body => _body.Value;

        public Expr Cond => _cond.Value;
    }
}
