// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
