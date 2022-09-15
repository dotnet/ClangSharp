// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed unsafe class TranslationUnitDecl : Decl, IDeclContext
{
    internal TranslationUnitDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TranslationUnit, CX_DeclKind.CX_DeclKind_TranslationUnit)
    {
    }
}
