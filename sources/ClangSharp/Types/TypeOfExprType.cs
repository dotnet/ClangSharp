// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypeOfExprType : Type
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Expr> _underlyingExpr;

        internal TypeOfExprType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_TypeOfExpr)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _underlyingExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.UnderlyingExpr));
        }

        public bool IsSugared => Handle.IsSugared;

        public Expr UnderlyingExpr => _underlyingExpr.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
