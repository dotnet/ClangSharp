// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public class MatrixType : Type
{
    private readonly ValueLazy<Type> _elementType;

    private protected MatrixType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _elementType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(handle.ElementType));
    }

    public Type ElementType => _elementType.Value;
}
