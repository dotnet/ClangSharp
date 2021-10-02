// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class BaseUsingDecl : NamedDecl
    {
        private protected BaseUsingDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastBaseUsing or < CX_DeclKind.CX_DeclKind_FirstBaseUsing)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }
    }
}
