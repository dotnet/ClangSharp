// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class UsingShadowDecl : NamedDecl, IRedeclarable<UsingShadowDecl>
{
    internal UsingShadowDecl(CXCursor handle) : this(handle, CXCursor_UnexposedDecl, CX_DeclKind_UsingShadow)
    {
    }

    private protected UsingShadowDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastUsingShadow or < CX_DeclKind_FirstUsingShadow)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
    }
}
