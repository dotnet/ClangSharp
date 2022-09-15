// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class UsingShadowDecl : NamedDecl, IRedeclarable<UsingShadowDecl>
{
    internal UsingShadowDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_UsingShadow)
    {
    }

    private protected UsingShadowDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastUsingShadow or < CX_DeclKind.CX_DeclKind_FirstUsingShadow)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
    }
}
