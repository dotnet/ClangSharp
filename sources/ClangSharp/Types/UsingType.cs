// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class UsingType : Type
{
    private readonly ValueLazy<UsingShadowDecl> _foundDecl;

    internal UsingType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_Using)
    {
        _foundDecl = new ValueLazy<UsingShadowDecl>(() => TranslationUnit.GetOrCreate<UsingShadowDecl>(Handle.Declaration));
    }

    public UsingShadowDecl FoundDecl => _foundDecl.Value;
}
