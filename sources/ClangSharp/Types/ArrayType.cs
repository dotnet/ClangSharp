// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class ArrayType : Type
{
    private readonly Lazy<Type> _elementType;

    private protected ArrayType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _elementType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ArrayElementType));
    }

    public Type ElementType => _elementType.Value;
}
