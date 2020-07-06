// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArrayInitLoopExpr : Expr
    {
        private readonly Lazy<OpaqueValueExpr> _commonExpr;
        private readonly Lazy<Expr> _subExpr;

        internal ArrayInitLoopExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr)
        {
            _commonExpr = new Lazy<OpaqueValueExpr>(() => TranslationUnit.GetOrCreate<OpaqueValueExpr>(Handle.CommonExpr));
            _subExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
        }

        public long ArraySize => Handle.ArraySize;

        public OpaqueValueExpr CommonExpr => _commonExpr.Value;

        public Expr SubExpr => _subExpr.Value;
    }
}
