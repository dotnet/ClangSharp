// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MaterializeTemporaryExpr : Expr
    {
        private readonly Lazy<LifetimeExtendedTemporaryDecl> _lifetimeExtendedTemporaryDecl;

        internal MaterializeTemporaryExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_MaterializeTemporaryExpr)
        {
            _lifetimeExtendedTemporaryDecl = new Lazy<LifetimeExtendedTemporaryDecl>(() => TranslationUnit.GetOrCreate<LifetimeExtendedTemporaryDecl>(Handle.Referenced));
        }

        public ValueDecl ExtendingDecl => LifetimeExtendedTemporaryDecl?.ExtendingDecl;

        public LifetimeExtendedTemporaryDecl LifetimeExtendedTemporaryDecl => _lifetimeExtendedTemporaryDecl.Value;

        public Expr SubExpr => (Expr)Children[0];
    }
}
