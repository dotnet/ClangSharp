// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class UnaryTransformType : Type
    {
        private readonly Lazy<Type> _baseType;
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _underlyingType;

        internal UnaryTransformType(CXType handle) : this(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_UnaryTransform)
        {
        }

        private protected UnaryTransformType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
            _baseType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.BaseType));
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
        }

        public Type BaseType => _baseType.Value;

        public bool IsSugared => Handle.IsSugared;

        public Type UnderlyingType => _underlyingType.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
