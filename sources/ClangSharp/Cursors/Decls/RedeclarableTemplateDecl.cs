// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class RedeclarableTemplateDecl : TemplateDecl, IRedeclarable<RedeclarableTemplateDecl>
    {
        private readonly Lazy<RedeclarableTemplateDecl> _instantiatedFromMemberTemplate;

        private protected RedeclarableTemplateDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastRedeclarableTemplate < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstRedeclarableTemplate))
            {
                throw new ArgumentException(nameof(handle));
            }

            _instantiatedFromMemberTemplate = new Lazy<RedeclarableTemplateDecl>(() => TranslationUnit.GetOrCreate<RedeclarableTemplateDecl>(Handle.SpecializedCursorTemplate));
        }

        public new RedeclarableTemplateDecl CanonicalDecl => (RedeclarableTemplateDecl)base.CanonicalDecl;

        public bool IsMemberSpecialization => Handle.IsMemberSpecialization;

        public RedeclarableTemplateDecl InstantiatedFromMemberTemplate => _instantiatedFromMemberTemplate.Value;
    }
}
