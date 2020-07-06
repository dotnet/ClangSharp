// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXThrowExpr : Expr
    {
        private readonly Lazy<Expr> _subExpr;

        internal CXXThrowExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXThrowExpr, CX_StmtClass.CX_StmtClass_CXXThrowExpr)
        {
            _subExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
        }

        public Expr SubExpr => _subExpr.Value;
    }
}
