// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NamespaceDecl : NamedDecl, IDeclContext, IRedeclarable<NamespaceDecl>
    {
        private readonly Lazy<IReadOnlyList<Decl>> _decls;

        internal NamespaceDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Namespace)
        {
            _decls = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.OfType<Decl>().ToList());
        }

        public bool IsAnonymousNamespace => Handle.IsAnonymous;

        public bool IsInlineNamespace => Handle.IsInlineNamespace;

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public IDeclContext LexicalParent => LexicalDeclContext;

        public IDeclContext Parent => DeclContext;
    }
}
