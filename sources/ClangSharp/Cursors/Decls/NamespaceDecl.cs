// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NamespaceDecl : NamedDecl, IDeclContext, IRedeclarable<NamespaceDecl>
    {
        private readonly Lazy<NamespaceDecl> _anonymousNamespace;
        private readonly Lazy<NamespaceDecl> _originalNamespace;

        internal NamespaceDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Namespace, CX_DeclKind.CX_DeclKind_Namespace)
        {
            _anonymousNamespace = new Lazy<NamespaceDecl>(() => TranslationUnit.GetOrCreate<NamespaceDecl>(Handle.GetSubDecl(0)));
            _originalNamespace = new Lazy<NamespaceDecl>(() => TranslationUnit.GetOrCreate<NamespaceDecl>(Handle.GetSubDecl(1)));
        }

        public NamespaceDecl AnonymousNamespace => _anonymousNamespace.Value;

        public new NamespaceDecl CanonicalDecl => (NamespaceDecl)base.CanonicalDecl;

        public bool IsAnonymousNamespace => Handle.IsAnonymous;

        public bool IsInline => Handle.IsInlineNamespace;

        public NamespaceDecl OriginalNamespace => _originalNamespace.Value;
    }
}
