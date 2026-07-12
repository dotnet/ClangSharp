// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class VectorType : Type
{
    private ValueLazy<VectorType, Type> _elementType;

    internal VectorType(CXType handle) : this(handle, CXType_Vector, CX_TypeClass_Vector)
    {
    }

    private protected unsafe VectorType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _elementType = new ValueLazy<VectorType, Type>(&ElementTypeFactory);
    }

    public Type ElementType => _elementType.GetValue(this);

    public long NumElements => Handle.NumElements;

    private static unsafe Type ElementTypeFactory(VectorType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ElementType);
}
