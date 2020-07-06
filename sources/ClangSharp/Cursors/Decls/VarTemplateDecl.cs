// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class VarTemplateDecl : RedeclarableTemplateDecl
    {
        internal VarTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_VarTemplate)
        {
        }

        public new VarTemplateDecl CanonicalDecl => (VarTemplateDecl)base.CanonicalDecl;

        public new VarTemplateDecl InstantiatedFromMemberTemplate => (VarTemplateDecl)base.InstantiatedFromMemberTemplate;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public new VarTemplateDecl MostRecentDecl => (VarTemplateDecl)base.MostRecentDecl;

        public new VarTemplateDecl PreviousDecl => (VarTemplateDecl)base.PreviousDecl;

        public new CXXRecordDecl TemplatedDecl => (CXXRecordDecl)base.TemplatedDecl;
    }
}
