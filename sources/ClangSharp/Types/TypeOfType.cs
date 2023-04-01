// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class TypeOfType : Type
{
    private readonly Lazy<Type> _underlyingType;

    internal TypeOfType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_TypeOf)
    {
        _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
    }

    public Type UnderlyingType => _underlyingType.Value;
}
