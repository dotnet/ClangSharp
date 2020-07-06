// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FullExpr : Expr
    {
        private readonly Lazy<Expr> _subExpr;

        private protected FullExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastFullExpr < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstFullExpr))
            {
                throw new ArgumentException(nameof(handle));
            }

            _subExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
        }

        public Expr SubExpr => _subExpr.Value;
    }
}
