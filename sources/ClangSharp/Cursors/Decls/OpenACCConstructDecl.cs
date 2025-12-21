// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class OpenACCConstructDecl : Decl
{
    private protected OpenACCConstructDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is not CX_DeclKind_OpenACCRoutine and not CX_DeclKind_OpenACCDeclare)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
    }
}
