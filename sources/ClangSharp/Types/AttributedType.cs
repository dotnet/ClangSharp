// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class AttributedType : Type
{
    private readonly ValueLazy<Type> _equivalentType;
    private readonly ValueLazy<Type> _modifiedType;

    internal AttributedType(CXType handle) : base(handle, CXType_Attributed, CX_TypeClass_Attributed)
    {
        _equivalentType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.EquivalentType));
        _modifiedType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ModifiedType));
    }

    public CX_AttrKind AttrKind => Handle.AttrKind;

    public Type EquivalentType => _equivalentType.Value;

    public Type ModifiedType => _modifiedType.Value;
}
