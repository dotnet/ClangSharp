// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TypeClass;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed class TemplateSpecializationType : Type
{
    private readonly LazyList<TemplateArgument> _templateArgs;
    private readonly Lazy<TemplateName> _templateName;

    internal TemplateSpecializationType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_TemplateSpecialization)
    {
        _templateArgs = LazyList.Create<TemplateArgument>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i))));
        _templateName = new Lazy<TemplateName>(() => TranslationUnit.GetOrCreate(Handle.TemplateName));
    }

    public Type? AliasedType => IsTypeAlias ? Desugar : null;

    public IReadOnlyList<TemplateArgument> Args => _templateArgs;

    [MemberNotNullWhen(true, nameof(AliasedType))]
    public bool IsTypeAlias => Handle.IsTypeAlias;

    public TemplateName TemplateName => _templateName.Value;
}
