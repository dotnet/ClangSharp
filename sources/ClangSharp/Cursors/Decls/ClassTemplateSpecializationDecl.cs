// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;
using System;

namespace ClangSharp
{
    public class ClassTemplateSpecializationDecl : CXXRecordDecl
    {
        private readonly Lazy<ClassTemplateDecl> _specializedTemplate;

        private protected ClassTemplateSpecializationDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _specializedTemplate = new Lazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.SpecializedCursorTemplate));
        }

        public ClassTemplateDecl SpecializedTemplate => _specializedTemplate.Value;
    }
}
