// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public class ArrayType : Type
{
    private ValueLazy<ArrayType, Type> _elementType;

    private protected unsafe ArrayType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _elementType = new ValueLazy<ArrayType, Type>(&ElementTypeFactory);
    }

    public Type ElementType => _elementType.GetValue(this);

    private static unsafe Type ElementTypeFactory(ArrayType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ArrayElementType);
}
