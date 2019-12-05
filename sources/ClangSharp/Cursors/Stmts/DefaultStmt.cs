// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DefaultStmt : SwitchCase
    {
        private readonly Lazy<Stmt> _subStmt;

        internal DefaultStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DefaultStmt, CX_StmtClass.CX_StmtClass_DefaultStmt)
        {
            _subStmt = new Lazy<Stmt>(() => Children.OfType<Stmt>().Single());
        }

        public Stmt SubStmt => _subStmt.Value;
    }
}
