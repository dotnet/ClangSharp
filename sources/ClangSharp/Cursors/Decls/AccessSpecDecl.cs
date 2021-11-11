// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AccessSpecDecl : Decl
    {
        internal AccessSpecDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXAccessSpecifier, CX_DeclKind.CX_DeclKind_AccessSpec)
        {
        }
    }
}
