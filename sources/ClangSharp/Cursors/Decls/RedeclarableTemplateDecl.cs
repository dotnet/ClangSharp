// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class RedeclarableTemplateDecl : TemplateDecl, IRedeclarable<RedeclarableTemplateDecl>
    {
        private readonly Lazy<RedeclarableTemplateDecl> _instantiatedFromMemberTemplate;

        private protected RedeclarableTemplateDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _instantiatedFromMemberTemplate = new Lazy<RedeclarableTemplateDecl>(() => TranslationUnit.GetOrCreate<RedeclarableTemplateDecl>(Handle.SpecializedCursorTemplate));
        }

        public bool IsMemberSpecialization => InstantiatedFromMemberTemplate != null;

        public RedeclarableTemplateDecl InstantiatedFromMemberTemplate => _instantiatedFromMemberTemplate.Value;
    }
}
