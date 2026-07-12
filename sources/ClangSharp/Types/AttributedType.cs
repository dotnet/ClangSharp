// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class AttributedType : Type
{
    private ValueLazy<AttributedType, Type> _equivalentType;
    private ValueLazy<AttributedType, Type> _modifiedType;

    internal unsafe AttributedType(CXType handle) : base(handle, CXType_Attributed, CX_TypeClass_Attributed)
    {
        _equivalentType = new ValueLazy<AttributedType, Type>(&EquivalentTypeFactory);
        _modifiedType = new ValueLazy<AttributedType, Type>(&ModifiedTypeFactory);
    }

    public CX_AttrKind AttrKind => Handle.AttrKind;

    public Type EquivalentType => _equivalentType.GetValue(this);

    public Type ModifiedType => _modifiedType.GetValue(this);

    private static unsafe Type ModifiedTypeFactory(AttributedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ModifiedType);

    private static unsafe Type EquivalentTypeFactory(AttributedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.EquivalentType);
}
