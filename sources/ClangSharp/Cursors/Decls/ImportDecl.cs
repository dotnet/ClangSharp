// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ImportDecl : Decl
{
    internal ImportDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ModuleImportDecl, CX_DeclKind.CX_DeclKind_Import)
    {
    }
}
