// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ComplexType : Type
{
    private readonly ValueLazy<Type> _elementType;

    internal ComplexType(CXType handle) : base(handle, CXType_Complex, CX_TypeClass_Complex)
    {
        _elementType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ElementType));
    }

    public Type ElementType => _elementType.Value;
}
