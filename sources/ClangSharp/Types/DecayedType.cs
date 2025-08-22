// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DecayedType : AdjustedType
{
    private readonly ValueLazy<Type> _decayedType;

    internal DecayedType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_Decayed)
    {
        _decayedType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DecayedType));
    }

    public Type GetDecayedType => _decayedType.Value;
}
