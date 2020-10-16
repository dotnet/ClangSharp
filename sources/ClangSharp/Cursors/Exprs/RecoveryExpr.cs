// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class RecoveryExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _subExpressions;

        internal RecoveryExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_RecoveryExpr)
        {
            _subExpressions = new Lazy<IReadOnlyList<Expr>>(() => {
                var numExprs = Handle.NumExprs;
                var exprs = new List<Expr>(numExprs);

                for (var index = 0; index < numExprs; index++)
                {
                    var expr = TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)index)));
                    exprs.Add(expr);
                }

                return exprs;
            });
        }

        public IReadOnlyList<Expr> SubExpressions => _subExpressions.Value;
    }
}
