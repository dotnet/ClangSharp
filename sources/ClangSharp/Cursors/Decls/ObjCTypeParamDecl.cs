// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCTypeParamDecl : TypedefNameDecl
{
    internal ObjCTypeParamDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_ObjCTypeParam)
    {
    }

    public uint Index => unchecked((uint)Handle.TemplateTypeParmIndex);
}
