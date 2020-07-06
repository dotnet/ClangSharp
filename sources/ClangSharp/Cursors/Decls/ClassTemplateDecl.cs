// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ClassTemplateDecl : RedeclarableTemplateDecl
    {
        internal ClassTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ClassTemplate, CX_DeclKind.CX_DeclKind_ClassTemplate)
        {
        }

        public new ClassTemplateDecl CanonicalDecl => (ClassTemplateDecl)base.CanonicalDecl;

        public new ClassTemplateDecl InstantiatedFromMemberTemplate => (ClassTemplateDecl)base.InstantiatedFromMemberTemplate;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public new ClassTemplateDecl MostRecentDecl => (ClassTemplateDecl)base.MostRecentDecl;

        public new ClassTemplateDecl PreviousDecl => (ClassTemplateDecl)base.PreviousDecl;

        public new CXXRecordDecl TemplatedDecl => (CXXRecordDecl)base.TemplatedDecl;
    }
}
