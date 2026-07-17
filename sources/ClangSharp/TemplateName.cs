// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TemplateNameKind;

namespace ClangSharp;

public sealed unsafe class TemplateName
{
    private ValueLazy<TemplateName, TemplateDecl> _asTemplateDecl;
    private ValueLazy<TemplateName, TemplateName> _underlying;
    private ValueLazy<TemplateName, TranslationUnit> _translationUnit;

    internal TemplateName(CX_TemplateName handle)
    {
        Handle = handle;

        _translationUnit = new ValueLazy<TemplateName, TranslationUnit>(&TranslationUnitFactory);
        _asTemplateDecl = new ValueLazy<TemplateName, TemplateDecl>(&AsTemplateDeclFactory);
        _underlying = new ValueLazy<TemplateName, TemplateName>(&UnderlyingFactory);
    }

    public TemplateDecl AsTemplateDecl => _asTemplateDecl.GetValue(this);

    public bool ContainsUnexpandedParameterPack => Handle.ContainsUnexpandedParameterPack;

    public CX_TemplateName Handle { get; }

    public bool IsDependent => Handle.IsDependent;

    public bool IsInstantiationDependent => Handle.IsInstantiationDependent;

    public bool IsNull => Handle.kind == CX_TNK_Invalid;

    public CX_TemplateNameKind Kind => Handle.kind;

    public TranslationUnit TranslationUnit => _translationUnit.GetValue(this);

    public TemplateName Underlying => _underlying.GetValue(this);

    private static unsafe TemplateDecl AsTemplateDeclFactory(TemplateName self) => self._translationUnit.GetValue(self).GetOrCreate<TemplateDecl>(self.Handle.AsTemplateDecl);

    private static unsafe TemplateName UnderlyingFactory(TemplateName self) => self._translationUnit.GetValue(self).GetOrCreate(self.Handle.Underlying);

    private static unsafe TranslationUnit TranslationUnitFactory(TemplateName self) => TranslationUnit.GetOrCreate(self.Handle.tu);
}
