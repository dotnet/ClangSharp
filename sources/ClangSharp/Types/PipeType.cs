// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class PipeType : Type
{
    private readonly ValueLazy<Type> _elementType;

    internal PipeType(CXType handle) : base(handle, CXType_Pipe, CX_TypeClass_Pipe)
    {
        _elementType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ElementType));
    }

    public Type ElementType => _elementType.Value;
}
