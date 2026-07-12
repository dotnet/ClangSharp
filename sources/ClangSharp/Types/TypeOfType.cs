// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class TypeOfType : Type
{
    private ValueLazy<TypeOfType, Type> _underlyingType;

    internal unsafe TypeOfType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_TypeOf)
    {
        _underlyingType = new ValueLazy<TypeOfType, Type>(&UnderlyingTypeFactory);
    }

    public Type UnderlyingType => _underlyingType.GetValue(this);

    private static unsafe Type UnderlyingTypeFactory(TypeOfType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.UnderlyingType);
}
