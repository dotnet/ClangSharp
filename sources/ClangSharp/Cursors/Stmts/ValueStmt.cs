// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ValueStmt : Stmt
    {
        private readonly Lazy<Expr> _exprStmt;

        private protected ValueStmt(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastValueStmt < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstValueStmt))
            {
                throw new ArgumentException(nameof(handle));
            }

            _exprStmt = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubStmt));
        }

        public Expr ExprStmt => _exprStmt.Value;
    }
}
