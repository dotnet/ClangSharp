// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TypeClass;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed class DependentTemplateSpecializationType : TypeWithKeyword
{
    private readonly LazyList<TemplateArgument> _templateArgs;

    internal unsafe DependentTemplateSpecializationType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_DependentTemplateSpecialization)
    {
        _templateArgs = LazyList.Create<TemplateArgument>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
    }

    public IReadOnlyList<TemplateArgument> Args => _templateArgs;

    private static unsafe TemplateArgument TemplateArgsFactory(object self, int i)
    {
        var @this = (DependentTemplateSpecializationType)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgument(unchecked((uint)i)));
    }
}
