// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
