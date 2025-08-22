// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class AdjustedType : Type
{
    private readonly ValueLazy<Type> _adjustedType;
    private readonly ValueLazy<Type> _originalType;

    internal AdjustedType(CXType handle) : this(handle, CXType_Unexposed, CX_TypeClass_Adjusted)
    {
    }

    private protected AdjustedType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _adjustedType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.AdjustedType));
        _originalType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.OriginalType));
    }

    public Type GetAdjustedType => _adjustedType.Value;

    public Type OriginalType => _originalType.Value;
}
