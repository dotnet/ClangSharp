// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public class DeducedType : Type
{
    private ValueLazy<DeducedType, Type> _deducedType;

    private protected unsafe DeducedType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _deducedType = new ValueLazy<DeducedType, Type>(&DeducedTypeFactory);
    }

    public Type GetDeducedType => _deducedType.GetValue(this);

    private static unsafe Type DeducedTypeFactory(DeducedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.DeducedType);
}
