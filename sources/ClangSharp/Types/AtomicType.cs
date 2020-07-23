// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AtomicType : Type
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _valueType;

        internal AtomicType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_Atomic)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _valueType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ValueType));
        }

        public bool IsSugared => Handle.IsSugared;

        public Type ValueType => _valueType.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
