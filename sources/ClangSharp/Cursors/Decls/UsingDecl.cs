// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UsingDecl : BaseUsingDecl, IMergeable<UsingDecl>
    {
        internal UsingDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UsingDeclaration, CX_DeclKind.CX_DeclKind_Using)
        {
        }
    }
}
