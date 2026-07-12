// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class PackExpansionType : Type
{
    private ValueLazy<PackExpansionType, Type> _pattern;

    internal unsafe PackExpansionType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_PackExpansion)
    {
        _pattern = new ValueLazy<PackExpansionType, Type>(&PatternFactory);
    }

    public Type Pattern => _pattern.GetValue(this);

    private static unsafe Type PatternFactory(PackExpansionType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.OriginalType);
}
