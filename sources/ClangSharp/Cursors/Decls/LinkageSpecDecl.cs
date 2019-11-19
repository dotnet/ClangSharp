// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LinkageSpecDecl : Decl, IDeclContext
    {
        private readonly Lazy<IReadOnlyList<Decl>> _decls;

        internal LinkageSpecDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind.CX_DeclKind_LinkageSpec)
        {
            if ((handle.Kind != CXCursorKind.CXCursor_LinkageSpec) && (handle.Kind != CXCursorKind.CXCursor_UnexposedDecl))
            {
                throw new ArgumentException(nameof(handle));
            }

            _decls = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.OfType<Decl>().ToList());
        }

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public CXLanguageKind Langage => Handle.Language;

        public IDeclContext LexicalParent => LexicalDeclContext;

        public IDeclContext Parent => DeclContext;
    }
}
