// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class UnaryTransformType : Type
{
    private ValueLazy<UnaryTransformType, Type> _baseType;
    private ValueLazy<UnaryTransformType, Type> _underlyingType;

    internal UnaryTransformType(CXType handle) : this(handle, CXType_Unexposed, CX_TypeClass_UnaryTransform)
    {
    }

    private protected unsafe UnaryTransformType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _baseType = new ValueLazy<UnaryTransformType, Type>(&BaseTypeFactory);
        _underlyingType = new ValueLazy<UnaryTransformType, Type>(&UnderlyingTypeFactory);
    }

    public Type BaseType => _baseType.GetValue(this);

    public Type UnderlyingType => _underlyingType.GetValue(this);

    private static unsafe Type UnderlyingTypeFactory(UnaryTransformType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.UnderlyingType);

    private static unsafe Type BaseTypeFactory(UnaryTransformType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.BaseType);
}
