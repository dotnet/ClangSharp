// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class MSGuidDecl : ValueDecl, IMergeable<MSGuidDecl>
{
    internal MSGuidDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_MSGuid)
    {
    }

    public Guid Value => Handle.GuidValue;
}
