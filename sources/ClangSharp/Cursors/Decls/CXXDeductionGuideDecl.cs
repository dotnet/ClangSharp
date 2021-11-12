// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXDeductionGuideDecl : FunctionDecl
    {
        private readonly Lazy<TemplateDecl> _deducedTemplate;

        internal CXXDeductionGuideDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_CXXDeductionGuide)
        {
            _deducedTemplate = new Lazy<TemplateDecl>(() => TranslationUnit.GetOrCreate<TemplateDecl>(handle.TemplatedDecl));
        }

        public bool IsExplicit => !Handle.IsImplicit;

        public TemplateDecl DeducedTemplate => _deducedTemplate.Value;
    }
}
