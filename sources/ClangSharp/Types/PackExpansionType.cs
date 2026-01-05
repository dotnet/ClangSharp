// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class PackExpansionType : Type
{
    private readonly ValueLazy<Type> _pattern;

    internal PackExpansionType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_PackExpansion)
    {
        _pattern = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.OriginalType));
    }

    public Type Pattern => _pattern.Value;
}
