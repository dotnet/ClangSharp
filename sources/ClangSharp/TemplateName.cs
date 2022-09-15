// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed unsafe class TemplateName
{
    private readonly Lazy<TemplateDecl> _asTemplateDecl;
    private readonly Lazy<TranslationUnit> _translationUnit;

    internal TemplateName(CX_TemplateName handle)
    {
        Handle = handle;

        _asTemplateDecl = new Lazy<TemplateDecl>(() => _translationUnit.Value.GetOrCreate<TemplateDecl>(Handle.AsTemplateDecl));
        _translationUnit = new Lazy<TranslationUnit>(() => TranslationUnit.GetOrCreate(Handle.tu));
    }

    public TemplateDecl AsTemplateDecl => _asTemplateDecl.Value;

    public CX_TemplateName Handle { get; }

    public TranslationUnit TranslationUnit => _translationUnit.Value;
}
