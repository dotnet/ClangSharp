// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ComplexType : Type
{
    private ValueLazy<ComplexType, Type> _elementType;

    internal unsafe ComplexType(CXType handle) : base(handle, CXType_Complex, CX_TypeClass_Complex)
    {
        _elementType = new ValueLazy<ComplexType, Type>(&ElementTypeFactory);
    }

    public Type ElementType => _elementType.GetValue(this);

    private static unsafe Type ElementTypeFactory(ComplexType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ElementType);
}
