// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class TemplateTypeParmType : Type
{
    private readonly Lazy<TemplateTypeParmDecl> _decl;

    internal TemplateTypeParmType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_TemplateTypeParm)
    {
        _decl = new Lazy<TemplateTypeParmDecl>(() => TranslationUnit.GetOrCreate<TemplateTypeParmDecl>(Handle.Declaration));
    }

    public TemplateTypeParmDecl Decl => _decl.Value;

    public uint Depth => unchecked((uint)Handle.Depth);

    public uint Index => unchecked((uint)Handle.Index);
}
