// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FriendTemplateDecl : Decl
    {
        private readonly Lazy<NamedDecl> _friendDecl;

        internal FriendTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_FriendTemplate)
        {
            _friendDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.FriendDecl));
        }

        public NamedDecl FriendDecl => _friendDecl.Value;
    }
}
