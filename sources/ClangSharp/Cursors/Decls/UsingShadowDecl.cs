// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class UsingShadowDecl : NamedDecl, IRedeclarable<UsingShadowDecl>
    {
        internal UsingShadowDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_UsingShadow)
        {
        }

        private protected UsingShadowDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastUsingShadow < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstUsingShadow))
            {
                throw new ArgumentException(nameof(handle));
            }
        }
    }
}
