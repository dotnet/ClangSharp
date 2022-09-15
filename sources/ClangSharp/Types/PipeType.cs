// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class PipeType : Type
{
    private readonly Lazy<Type> _elementType;

    internal PipeType(CXType handle) : base(handle, CXTypeKind.CXType_Pipe, CX_TypeClass.CX_TypeClass_Pipe)
    {
        _elementType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ElementType));
    }

    public Type ElementType => _elementType.Value;
}
