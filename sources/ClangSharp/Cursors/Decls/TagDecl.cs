// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public unsafe class TagDecl : TypeDecl, IDeclContext, IRedeclarable<TagDecl>
    {
        private readonly Lazy<IReadOnlyList<Decl>> _decls;
        private readonly Lazy<TagDecl> _definition;

        private protected TagDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _decls = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.OfType<Decl>().ToList());
            _definition = new Lazy<TagDecl>(() => TranslationUnit.GetOrCreate<TagDecl>(Handle.Definition));
        }

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public TagDecl Definition => _definition.Value;

        public bool IsClass => CursorKind == CXCursorKind.CXCursor_ClassDecl;

        public bool IsEnum => CursorKind == CXCursorKind.CXCursor_EnumDecl;

        public bool IsStruct => CursorKind == CXCursorKind.CXCursor_StructDecl;

        public bool IsUnion => CursorKind == CXCursorKind.CXCursor_UnionDecl;

        public IDeclContext LexicalParent => LexicalDeclContext;

        public IDeclContext Parent => DeclContext;
    }
}
