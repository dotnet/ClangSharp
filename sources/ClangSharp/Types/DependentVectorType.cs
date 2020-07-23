// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentVectorType : Type
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _elementType;
        private readonly Lazy<Expr> _sizeExpr;

        internal DependentVectorType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_DependentVector)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _elementType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ElementType));
            _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SizeExpr));
        }

        public Type ElementType => _elementType.Value;

        public bool IsSugared => Handle.IsSugared;

        public long Size => Handle.ArraySize;

        public Expr SizeExpr => _sizeExpr.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
