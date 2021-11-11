// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DecltypeType : Type
    {
        private readonly Lazy<Expr> _underlyingExpr;
        private readonly Lazy<Type> _underlyingType;

        internal DecltypeType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_Decltype)
        {
            _underlyingExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.UnderlyingExpr));
            _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
        }

        public Expr UnderlyingExpr => _underlyingExpr.Value;

        public Type UnderlyingType => _underlyingType.Value;
    }
}
