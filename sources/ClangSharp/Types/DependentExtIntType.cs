// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentExtIntType : Type
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Expr> _numBitsExpr;

        internal DependentExtIntType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_DependentExtInt)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _numBitsExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.NumBitsExpr));
        }

        public bool IsSigned => Handle.IsSigned;

        public bool IsSugared => Handle.IsSugared;

        public bool IsUnsigned => Handle.IsUnsigned;

        public Expr NumBitsExpr => _numBitsExpr.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
