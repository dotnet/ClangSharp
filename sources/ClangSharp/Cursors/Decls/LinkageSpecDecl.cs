// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class LinkageSpecDecl : Decl, IDeclContext
{
    internal LinkageSpecDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind_LinkageSpec)
    {
        if (handle.Kind is not CXCursor_LinkageSpec and not CXCursor_UnexposedDecl)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
    }

    public CXLanguageKind Langage => Handle.Language;
}
