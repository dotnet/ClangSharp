// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class UnresolvedUsingType : Type
{
    private readonly ValueLazy<UnresolvedUsingTypenameDecl> _decl;

    internal UnresolvedUsingType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_UnresolvedUsing)
    {
        _decl = new ValueLazy<UnresolvedUsingTypenameDecl>(() => TranslationUnit.GetOrCreate<UnresolvedUsingTypenameDecl>(Handle.Declaration));
    }

    public UnresolvedUsingTypenameDecl Decl => _decl.Value;
}
