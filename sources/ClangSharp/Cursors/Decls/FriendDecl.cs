// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FriendDecl : Decl
    {
        internal FriendDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FriendDecl, CX_DeclKind.CX_DeclKind_Friend)
        {
        }
    }
}
