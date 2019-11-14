// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class NamedDecl : Decl
    {
        private protected NamedDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastNamed < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstNamed))
            {
                throw new ArgumentException(nameof(handle));
            }
        }

        public CXLinkageKind LinkageInternal => Handle.Linkage;

        public string Name => Spelling;

        public CXVisibilityKind Visibility => Handle.Visibility;
    }
}
