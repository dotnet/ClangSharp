// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ComplexType : Type
    {
        private readonly Lazy<Type> _elementType;

        internal ComplexType(CXType handle) : base(handle, CXTypeKind.CXType_Complex, CX_TypeClass.CX_TypeClass_Complex)
        {
            _elementType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ElementType));
        }

        public Type ElementType => _elementType.Value;
    }
}
