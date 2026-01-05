// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed unsafe class TemplateName
{
    private readonly ValueLazy<TemplateDecl> _asTemplateDecl;
    private readonly ValueLazy<TranslationUnit> _translationUnit;

    internal TemplateName(CX_TemplateName handle)
    {
        Handle = handle;

        _translationUnit = new ValueLazy<TranslationUnit>(() => TranslationUnit.GetOrCreate(Handle.tu));
        _asTemplateDecl = new ValueLazy<TemplateDecl>(() => _translationUnit.Value.GetOrCreate<TemplateDecl>(Handle.AsTemplateDecl));
    }

    public TemplateDecl AsTemplateDecl => _asTemplateDecl.Value;

    public CX_TemplateName Handle { get; }

    public TranslationUnit TranslationUnit => _translationUnit.Value;
}
