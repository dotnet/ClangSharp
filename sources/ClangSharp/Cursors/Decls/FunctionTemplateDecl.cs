// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionTemplateDecl : RedeclarableTemplateDecl
    {
        internal FunctionTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FunctionTemplate, CX_DeclKind.CX_DeclKind_FunctionTemplate)
        {
        }

        public new FunctionTemplateDecl CanonicalDecl => (FunctionTemplateDecl)base.CanonicalDecl;

        public new FunctionTemplateDecl InstantiatedFromMemberTemplate => (FunctionTemplateDecl)base.InstantiatedFromMemberTemplate;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public new FunctionTemplateDecl MostRecentDecl => (FunctionTemplateDecl)base.MostRecentDecl;

        public new FunctionTemplateDecl PreviousDecl => (FunctionTemplateDecl)base.PreviousDecl;

        public new FunctionDecl TemplatedDecl => (FunctionDecl)base.TemplatedDecl;
    }
}
