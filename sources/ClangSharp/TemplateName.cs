// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed unsafe class TemplateName
{
    private ValueLazy<TemplateName, TemplateDecl> _asTemplateDecl;
    private ValueLazy<TemplateName, TranslationUnit> _translationUnit;

    internal TemplateName(CX_TemplateName handle)
    {
        Handle = handle;

        _translationUnit = new ValueLazy<TemplateName, TranslationUnit>(&TranslationUnitFactory);
        _asTemplateDecl = new ValueLazy<TemplateName, TemplateDecl>(&AsTemplateDeclFactory);
    }

    public TemplateDecl AsTemplateDecl => _asTemplateDecl.GetValue(this);

    public CX_TemplateName Handle { get; }

    public TranslationUnit TranslationUnit => _translationUnit.GetValue(this);

    private static unsafe TemplateDecl AsTemplateDeclFactory(TemplateName self) => self._translationUnit.GetValue(self).GetOrCreate<TemplateDecl>(self.Handle.AsTemplateDecl);

    private static unsafe TranslationUnit TranslationUnitFactory(TemplateName self) => TranslationUnit.GetOrCreate(self.Handle.tu);
}
