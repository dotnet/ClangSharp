// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class CXXDeductionGuideDecl : FunctionDecl
{
    private ValueLazy<CXXDeductionGuideDecl, TemplateDecl> _deducedTemplate;

    internal unsafe CXXDeductionGuideDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_CXXDeductionGuide)
    {
        _deducedTemplate = new ValueLazy<CXXDeductionGuideDecl, TemplateDecl>(&DeducedTemplateFactory);
    }

    public bool IsExplicit => !Handle.IsImplicit;

    public TemplateDecl DeducedTemplate => _deducedTemplate.GetValue(this);

    private static unsafe TemplateDecl DeducedTemplateFactory(CXXDeductionGuideDecl self) => self.TranslationUnit.GetOrCreate<TemplateDecl>(self.Handle.TemplatedDecl);
}
