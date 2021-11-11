// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateParamObjectDecl : ValueDecl, IMergeable<TemplateParamObjectDecl>
    {
        internal TemplateParamObjectDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_TemplateParamObject)
        {
        }

        public new TemplateParamObjectDecl CanonicalDecl => (TemplateParamObjectDecl)base.CanonicalDecl;
    }
}
