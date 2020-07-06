// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class InitListExpr : Expr
    {
        private readonly Lazy<Expr> _arrayFiller;
        private readonly Lazy<FieldDecl> _initializedFieldInUnion;
        private readonly Lazy<IReadOnlyList<Expr>> _inits;

        internal InitListExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_InitListExpr, CX_StmtClass.CX_StmtClass_InitListExpr)
        {
            _arrayFiller = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
            _initializedFieldInUnion = new Lazy<FieldDecl>(() => TranslationUnit.GetOrCreate<FieldDecl>(Handle.Referenced));
            _inits = new Lazy<IReadOnlyList<Expr>>(() => {
                var initCount = Handle.NumExprs;
                var inits = new List<Expr>(initCount);

                for (int i = 0; i < initCount; i++)
                {
                    var init = TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i)));
                    inits.Add(init);
                }

                return inits;
            });
        }

        public IReadOnlyList<Stmt> Inits => _inits.Value;
    }
}
