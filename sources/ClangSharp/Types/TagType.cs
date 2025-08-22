// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class TagType : Type
{
    private readonly ValueLazy<TagDecl> _decl;

    private protected TagType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _decl = new ValueLazy<TagDecl>(() => TranslationUnit.GetOrCreate<TagDecl>(Handle.Declaration));
    }

    public TagDecl Decl => _decl.Value;
}
