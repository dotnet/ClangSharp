// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class DeducedType : Type
    {
        private readonly Lazy<Type> _deducedType;
        private readonly Lazy<Type> _desugaredType;

        private protected DeducedType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
            _deducedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DeducedType));
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
        }

        public Type GetDeducedType => _deducedType.Value;

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;
    }
}
