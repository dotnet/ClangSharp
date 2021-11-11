// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ArrayType : Type
    {
        private readonly Lazy<Type> _elementType;

        private protected ArrayType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
            _elementType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ArrayElementType));
        }

        public Type ElementType => _elementType.Value;
    }
}
