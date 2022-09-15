// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class TypeAliasDecl : TypedefNameDecl
{
    internal TypeAliasDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TypeAliasDecl, CX_DeclKind.CX_DeclKind_TypeAlias)
    {
    }
}
