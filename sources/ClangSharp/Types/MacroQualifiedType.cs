// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class MacroQualifiedType : Type
{
    private readonly Lazy<Type> _modifiedType;
    private readonly Lazy<Type> _underlyingType;

    internal MacroQualifiedType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_MacroQualified)
    {
        _modifiedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ModifiedType));
        _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
    }

    public Type ModifiedType => _modifiedType.Value;

    public Type UnderlyingType => _underlyingType.Value;
}
