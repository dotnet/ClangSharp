// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UsingEnumDecl : BaseUsingDecl, IMergeable<UsingEnumDecl>
    {
        private readonly Lazy<EnumDecl> _enumDecl;

        internal UsingEnumDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_UsingEnum)
        {
            _enumDecl = new Lazy<EnumDecl>(() => TranslationUnit.GetOrCreate<EnumDecl>(Handle.Definition));
        }

        public new UsingEnumDecl CanonicalDecl => (UsingEnumDecl)base.CanonicalDecl;

        public EnumDecl EnumDecl => _enumDecl.Value;
    }
}
