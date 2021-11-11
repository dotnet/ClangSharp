// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PseudoObjectExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _semantics;

        internal PseudoObjectExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_PseudoObjectExpr)
        {
            Debug.Assert(NumChildren >= 1);
            _semantics = new Lazy<IReadOnlyList<Expr>>(() => Children.Skip(1).Cast<Expr>().ToList());
        }

        public uint NumSemanticExprs => NumChildren - 1;

        public Expr ResultExpr => (ResultExprIndex == 0) ? null : (Expr)Children[unchecked((int)ResultExprIndex)];

        public uint ResultExprIndex => unchecked((uint)Handle.ResultIndex);

        public IReadOnlyList<Expr> Semantics => _semantics.Value;

        public Expr SyntacticForm => (Expr)Children[0];
    }
}
