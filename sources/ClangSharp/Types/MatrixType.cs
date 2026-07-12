// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public class MatrixType : Type
{
    private ValueLazy<MatrixType, Type> _elementType;

    private protected unsafe MatrixType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _elementType = new ValueLazy<MatrixType, Type>(&ElementTypeFactory);
    }

    public Type ElementType => _elementType.GetValue(this);

    private static unsafe Type ElementTypeFactory(MatrixType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ElementType);
}
