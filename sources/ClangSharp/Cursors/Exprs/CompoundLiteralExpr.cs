// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CompoundLiteralExpr : Expr
    {
        private readonly Lazy<Expr> _initializer;

        internal CompoundLiteralExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CompoundLiteralExpr, CX_StmtClass.CX_StmtClass_CompoundLiteralExpr)
        {
            _initializer = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.InitExpr));
        }

        public Expr Initializer => _initializer.Value;
    }
}
