// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class NamedDecl : Decl
{
    private readonly Lazy<NamedDecl> _underlyingDecl;

    private protected NamedDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastNamed or < CX_DeclKind.CX_DeclKind_FirstNamed)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _underlyingDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.UnderlyingDecl));
    }

    public CXLinkageKind LinkageInternal => Handle.Linkage;

    public new NamedDecl MostRecentDecl => (NamedDecl)base.MostRecentDecl;

    public string Name => Spelling;

    public NamedDecl UnderlyingDecl => _underlyingDecl.Value;

    public CXVisibilityKind Visibility => Handle.Visibility;
}
