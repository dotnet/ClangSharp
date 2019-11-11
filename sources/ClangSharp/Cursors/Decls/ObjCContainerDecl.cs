// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ObjCContainerDecl : NamedDecl, IDeclContext
    {
        private readonly Lazy<IReadOnlyList<Decl>> _decls;

        private protected ObjCContainerDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _decls = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.Where((cursor) => cursor is Decl).Cast<Decl>().ToList());
        }

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public IDeclContext LexicalParent => LexicalDeclContext;

        public IDeclContext Parent => DeclContext;
    }
}
