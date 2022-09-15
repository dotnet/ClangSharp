// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class UnaryTransformType : Type
{
    private readonly Lazy<Type> _baseType;
    private readonly Lazy<Type> _underlyingType;

    internal UnaryTransformType(CXType handle) : this(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_UnaryTransform)
    {
    }

    private protected UnaryTransformType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _baseType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.BaseType));
        _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
    }

    public Type BaseType => _baseType.Value;

    public Type UnderlyingType => _underlyingType.Value;
}
