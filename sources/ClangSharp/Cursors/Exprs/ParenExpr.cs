// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ParenExpr : Expr
    {
        private readonly Lazy<Expr> _subExpr;

        internal ParenExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ParenExpr)
        {
            _subExpr = new Lazy<Expr>(() => Children.Where((cursor) => cursor is Expr).Cast<Expr>().Single());
        }

        public Expr SubExpr => _subExpr.Value;
    }
}
