// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class TemplateSpecializationType : Type
{
    private readonly Lazy<IReadOnlyList<TemplateArgument>> _templateArgs;
    private readonly Lazy<TemplateName> _templateName;

    internal TemplateSpecializationType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_TemplateSpecialization)
    {
        _templateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
            var templateArgCount = Handle.NumTemplateArguments;
            var templateArgs = new List<TemplateArgument>(templateArgCount);

            for (var i = 0; i < templateArgCount; i++)
            {
                var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                templateArgs.Add(templateArg);
            }

            return templateArgs;
        });

        _templateName = new Lazy<TemplateName>(() => TranslationUnit.GetOrCreate(Handle.TemplateName));
    }

    public Type? AliasedType => IsTypeAlias ? Desugar : null;

    public IReadOnlyList<TemplateArgument> Args => _templateArgs.Value;

    [MemberNotNullWhen(true, nameof(AliasedType))]
    public bool IsTypeAlias => Handle.IsTypeAlias;

    public TemplateName TemplateName => _templateName.Value;
}
