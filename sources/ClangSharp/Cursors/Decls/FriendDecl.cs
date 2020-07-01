// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FriendDecl : Decl
    {
        private readonly Lazy<NamedDecl> _friendNamedDecl;

        internal FriendDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FriendDecl, CX_DeclKind.CX_DeclKind_Friend)
        {
            _friendNamedDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.FriendDecl));
        }

        public NamedDecl FriendNamedDecl => _friendNamedDecl.Value;

        public bool IsUnsupportedFriend => Handle.IsUnsupportedFriend;
    }
}
