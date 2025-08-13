// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class VectorType : Type
{
    private readonly ValueLazy<Type> _elementType;

    internal VectorType(CXType handle) : this(handle, CXType_Vector, CX_TypeClass_Vector)
    {
    }

    private protected VectorType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _elementType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ElementType));
    }

    public Type ElementType => _elementType.Value;

    public long NumElements => Handle.NumElements;
}
