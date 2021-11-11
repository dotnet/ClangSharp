// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypeOfExprType : Type
    {
        private readonly Lazy<Expr> _underlyingExpr;

        internal TypeOfExprType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_TypeOfExpr)
        {
            _underlyingExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.UnderlyingExpr));
        }

        public Expr UnderlyingExpr => _underlyingExpr.Value;
    }
}
