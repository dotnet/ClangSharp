// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ImportDecl : Decl
{
    internal ImportDecl(CXCursor handle) : base(handle, CXCursor_ModuleImportDecl, CX_DeclKind_Import)
    {
    }
}
