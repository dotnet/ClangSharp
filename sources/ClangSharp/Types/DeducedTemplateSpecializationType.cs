// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DeducedTemplateSpecializationType : DeducedType
{
    private ValueLazy<DeducedTemplateSpecializationType, TemplateName> _templateName;

    internal unsafe DeducedTemplateSpecializationType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_DeducedTemplateSpecialization)
    {
        _templateName = new ValueLazy<DeducedTemplateSpecializationType, TemplateName>(&TemplateNameFactory);
    }

    public TemplateName TemplateName => _templateName.GetValue(this);

    private static unsafe TemplateName TemplateNameFactory(DeducedTemplateSpecializationType self) => self.TranslationUnit.GetOrCreate(self.Handle.TemplateName);
}
