// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class MacroQualifiedType : Type
{
    private ValueLazy<MacroQualifiedType, Type> _modifiedType;
    private ValueLazy<MacroQualifiedType, Type> _underlyingType;

    internal unsafe MacroQualifiedType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_MacroQualified)
    {
        _modifiedType = new ValueLazy<MacroQualifiedType, Type>(&ModifiedTypeFactory);
        _underlyingType = new ValueLazy<MacroQualifiedType, Type>(&UnderlyingTypeFactory);
    }

    public Type ModifiedType => _modifiedType.GetValue(this);

    public Type UnderlyingType => _underlyingType.GetValue(this);

    private static unsafe Type UnderlyingTypeFactory(MacroQualifiedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.UnderlyingType);

    private static unsafe Type ModifiedTypeFactory(MacroQualifiedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ModifiedType);
}
