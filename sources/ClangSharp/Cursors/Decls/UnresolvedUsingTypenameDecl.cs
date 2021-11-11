// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnresolvedUsingTypenameDecl : TypeDecl, IMergeable<UnresolvedUsingTypenameDecl>
    {
        internal UnresolvedUsingTypenameDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UsingDeclaration, CX_DeclKind.CX_DeclKind_UnresolvedUsingTypename)
        {
        }
    }
}
