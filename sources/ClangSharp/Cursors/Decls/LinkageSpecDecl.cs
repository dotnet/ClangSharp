// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LinkageSpecDecl : Decl, IDeclContext
    {
        internal LinkageSpecDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind.CX_DeclKind_LinkageSpec)
        {
            if ((handle.Kind != CXCursorKind.CXCursor_LinkageSpec) && (handle.Kind != CXCursorKind.CXCursor_UnexposedDecl))
            {
                throw new ArgumentException(nameof(handle));
            }
        }

        public CXLanguageKind Langage => Handle.Language;
    }
}
