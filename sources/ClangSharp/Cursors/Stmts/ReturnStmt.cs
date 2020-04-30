// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ReturnStmt : Stmt
    {
        private readonly Lazy<Expr> _retValue;

        internal ReturnStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ReturnStmt, CX_StmtClass.CX_StmtClass_ReturnStmt)
        {
            _retValue = new Lazy<Expr>(() => Children.OfType<Expr>().SingleOrDefault());
        }

        public Expr RetValue => _retValue.Value;
    }
}
