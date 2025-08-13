// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class UsingEnumDecl : BaseUsingDecl, IMergeable<UsingEnumDecl>
{
    private readonly ValueLazy<EnumDecl> _enumDecl;

    internal UsingEnumDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_UsingEnum)
    {
        _enumDecl = new ValueLazy<EnumDecl>(() => TranslationUnit.GetOrCreate<EnumDecl>(Handle.Definition));
    }

    public new UsingEnumDecl CanonicalDecl => (UsingEnumDecl)base.CanonicalDecl;

    public EnumDecl EnumDecl => _enumDecl.Value;
}
