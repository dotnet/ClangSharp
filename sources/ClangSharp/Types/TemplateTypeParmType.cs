// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class TemplateTypeParmType : Type
{
    private readonly Lazy<TemplateTypeParmDecl> _decl;

    internal TemplateTypeParmType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_TemplateTypeParm)
    {
        _decl = new Lazy<TemplateTypeParmDecl>(() => TranslationUnit.GetOrCreate<TemplateTypeParmDecl>(Handle.Declaration));
    }

    public TemplateTypeParmDecl Decl => _decl.Value;

    public uint Depth => unchecked((uint)Handle.Depth);

    public uint Index => unchecked((uint)Handle.Index);
}
