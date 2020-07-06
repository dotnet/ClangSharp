// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXDefaultInitExpr : Expr
    {
        private Lazy<Expr> _expr;
        private Lazy<FieldDecl> _field;
        private Lazy<IDeclContext> _usedContext;

        internal CXXDefaultInitExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_CXXDefaultInitExpr)
        {
            _expr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
            _field = new Lazy<FieldDecl>(() => TranslationUnit.GetOrCreate<FieldDecl>(Handle.Referenced));
            _usedContext = new Lazy<IDeclContext>(() => TranslationUnit.GetOrCreate<Decl>(Handle.UsedContext) as IDeclContext);
        }

        public Expr Expr => _expr.Value;

        public FieldDecl Field => _field.Value;

        public IDeclContext UsedContext => _usedContext.Value;
    }
}
