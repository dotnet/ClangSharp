// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ParenListExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _exprs;

        internal ParenListExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ParenListExpr)
        {
            _exprs = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
        }

        public IReadOnlyList<Expr> Exprs => _exprs.Value;

        public uint NumExprs => NumChildren;
    }
}
