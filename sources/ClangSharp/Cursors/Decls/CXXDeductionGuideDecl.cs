// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class CXXDeductionGuideDecl : FunctionDecl
{
    private readonly Lazy<TemplateDecl> _deducedTemplate;

    internal CXXDeductionGuideDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_CXXDeductionGuide)
    {
        _deducedTemplate = new Lazy<TemplateDecl>(() => TranslationUnit.GetOrCreate<TemplateDecl>(handle.TemplatedDecl));
    }

    public bool IsExplicit => !Handle.IsImplicit;

    public TemplateDecl DeducedTemplate => _deducedTemplate.Value;
}
