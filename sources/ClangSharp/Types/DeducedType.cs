// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class DeducedType : Type
{
    private readonly ValueLazy<Type> _deducedType;

    private protected DeducedType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _deducedType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DeducedType));
    }

    public Type GetDeducedType => _deducedType.Value;
}
