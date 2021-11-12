// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AttributedType : Type
    {
        private readonly Lazy<Type> _equivalentType;
        private readonly Lazy<Type> _modifiedType;

        internal AttributedType(CXType handle) : base(handle, CXTypeKind.CXType_Attributed, CX_TypeClass.CX_TypeClass_Attributed)
        {
            _equivalentType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.EquivalentType));
            _modifiedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ModifiedType));
        }

        public CX_AttrKind AttrKind => Handle.AttrKind;

        public Type EquivalentType => _equivalentType.Value;

        public Type ModifiedType => _modifiedType.Value;
    }
}
