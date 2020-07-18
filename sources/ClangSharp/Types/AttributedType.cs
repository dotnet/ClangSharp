// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AttributedType : Type
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _equivalentType;
        private readonly Lazy<Type> _modifiedType;

        internal AttributedType(CXType handle) : base(handle, CXTypeKind.CXType_Attributed, CX_TypeClass.CX_TypeClass_Attributed)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _equivalentType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.EquivalentType));
            _modifiedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ModifiedType));
        }

        public CX_AttrKind AttrKind => Handle.AttrKind;

        public Type EquivalentType => _equivalentType.Value;

        public Type ModifiedType => _modifiedType.Value;

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;
    }
}
