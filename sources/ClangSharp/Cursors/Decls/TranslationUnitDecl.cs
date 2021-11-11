// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed unsafe class TranslationUnitDecl : Decl, IDeclContext
    {
        internal TranslationUnitDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TranslationUnit, CX_DeclKind.CX_DeclKind_TranslationUnit)
        {
        }
    }
}
