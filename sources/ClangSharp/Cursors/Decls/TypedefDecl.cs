// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypedefDecl : TypedefNameDecl
    {
        internal TypedefDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TypedefDecl, CX_DeclKind.CX_DeclKind_Typedef)
        {
        }
    }
}
