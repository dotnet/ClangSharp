// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MacroQualifiedType : Type
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _modifiedType;
        private readonly Lazy<Type> _underlyingType;

        internal MacroQualifiedType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_MacroQualified)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _modifiedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ModifiedType));
            _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
        }

        public bool IsSugared => Handle.IsSugared;

        public Type ModifiedType => _modifiedType.Value;

        public Type UnderlyingType => _underlyingType.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
