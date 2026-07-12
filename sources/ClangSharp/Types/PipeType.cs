// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class PipeType : Type
{
    private ValueLazy<PipeType, Type> _elementType;

    internal unsafe PipeType(CXType handle) : base(handle, CXType_Pipe, CX_TypeClass_Pipe)
    {
        _elementType = new ValueLazy<PipeType, Type>(&ElementTypeFactory);
    }

    public Type ElementType => _elementType.GetValue(this);

    private static unsafe Type ElementTypeFactory(PipeType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ElementType);
}
