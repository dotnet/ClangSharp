// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCTypeParamDecl : TypedefNameDecl
    {
        internal ObjCTypeParamDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_ObjCTypeParam)
        {
        }

        public uint Index => unchecked((uint)Handle.TemplateTypeParmIndex);
    }
}
