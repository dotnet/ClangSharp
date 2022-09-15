// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class DecayedType : AdjustedType
{
    private readonly Lazy<Type> _decayedType;

    internal DecayedType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_Decayed)
    {
        _decayedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DecayedType));
    }

    public Type GetDecayedType => _decayedType.Value;
}
