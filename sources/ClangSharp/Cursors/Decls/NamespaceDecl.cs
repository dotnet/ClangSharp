// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NamespaceDecl : NamedDecl, IDeclContext, IRedeclarable<NamespaceDecl>
    {
        internal NamespaceDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Namespace, CX_DeclKind.CX_DeclKind_Namespace)
        {
        }

        public new NamespaceDecl CanonicalDecl => (NamespaceDecl)base.CanonicalDecl;

        public bool IsAnonymousNamespace => Handle.IsAnonymous;

        public bool IsInline => Handle.IsInlineNamespace;
    }
}
