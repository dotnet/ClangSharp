// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public class NamedDecl : Decl
    {
        private protected NamedDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        public CXLinkageKind LinkageInternal => Handle.Linkage;

        public string Name => Spelling;

        public CXVisibilityKind Visibility => Handle.Visibility;
    }
}
