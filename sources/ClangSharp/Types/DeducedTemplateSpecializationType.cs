// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class DeducedTemplateSpecializationType : DeducedType
{
    private readonly Lazy<TemplateName> _templateName;

    internal DeducedTemplateSpecializationType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_DeducedTemplateSpecialization)
    {
        _templateName = new Lazy<TemplateName>(() => TranslationUnit.GetOrCreate(Handle.TemplateName));
    }

    public TemplateName TemplateName => _templateName.Value;
}
