// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class EnumConstantDecl : ValueDecl, IMergeable<EnumConstantDecl>
    {
        private readonly Lazy<Expr> _initExpr;

        internal EnumConstantDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_EnumConstantDecl)
        {
            _initExpr = new Lazy<Expr>(() => CursorChildren.OfType<Expr>().SingleOrDefault());
        }

        public Expr InitExpr => _initExpr.Value;

        public long InitVal => Handle.EnumConstantDeclValue;
    }
}
