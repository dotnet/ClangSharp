// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CaseStmt : SwitchCase
    {
        private readonly Lazy<Expr> _lhs;
        private readonly Lazy<Stmt> _subStmt;

        internal CaseStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CaseStmt, CX_StmtClass.CX_StmtClass_CaseStmt)
        {
            _lhs = new Lazy<Expr>(() => Children.OfType<Expr>().ElementAt(0));
            _subStmt = new Lazy<Stmt>(() => Children.OfType<Stmt>().ElementAt(1));
        }

        public Expr LHS => _lhs.Value;

        public Stmt SubStmt => _subStmt.Value;
    }
}
