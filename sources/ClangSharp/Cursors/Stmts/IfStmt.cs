// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IfStmt : Stmt
    {
        private readonly Lazy<Expr> _cond;
        private readonly Lazy<Stmt> _then;
        private readonly Lazy<Stmt> _else;

        internal IfStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_IfStmt, CX_StmtClass.CX_StmtClass_IfStmt)
        {
            _cond = new Lazy<Expr>(() => Children.OfType<Expr>().ElementAt(0));
            _then = new Lazy<Stmt>(() => Children.OfType<Stmt>().ElementAt(1));
            _else = new Lazy<Stmt>(() => Children.OfType<Stmt>().ElementAtOrDefault(2));
        }

        public Expr Cond => _cond.Value;

        public Stmt Then => _then.Value;

        public Stmt Else => _else.Value;
    }
}
