// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

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
