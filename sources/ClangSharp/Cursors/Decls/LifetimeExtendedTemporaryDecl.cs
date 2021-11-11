// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LifetimeExtendedTemporaryDecl : Decl, IMergeable<LifetimeExtendedTemporaryDecl>
    {
        private readonly Lazy<ValueDecl> _extendingDecl;
        private readonly Lazy<Expr> _temporaryExpr;

        internal LifetimeExtendedTemporaryDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_LifetimeExtendedTemporary)
        {
            _extendingDecl = new Lazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.GetSubDecl(1)));
            _temporaryExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
        }

        public ValueDecl ExtendingDecl => _extendingDecl.Value;

        public Expr TemporaryExpr => _temporaryExpr.Value;
    }
}
