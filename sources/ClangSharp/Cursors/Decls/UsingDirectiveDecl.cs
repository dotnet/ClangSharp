// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class UsingDirectiveDecl : NamedDecl
{
    internal UsingDirectiveDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UsingDirective, CX_DeclKind.CX_DeclKind_UsingDirective)
    {
    }
}
