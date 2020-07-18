// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class AdjustedType : Type
    {
        private readonly Lazy<Type> _adjustedType;
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _originalType;

        internal AdjustedType(CXType handle) : this(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_Adjusted)
        {
        }

        private protected AdjustedType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
            _adjustedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.AdjustedType));
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _originalType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.OriginalType));
        }

        public Type GetAdjustedType => _adjustedType.Value;

        public bool IsSugared => Handle.IsSugared;

        public Type OriginalType => _originalType.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
