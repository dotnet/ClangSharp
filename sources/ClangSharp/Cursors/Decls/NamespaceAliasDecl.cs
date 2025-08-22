// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class NamespaceAliasDecl : NamedDecl, IRedeclarable<NamespaceDecl>
{
    private readonly ValueLazy<NamedDecl> _aliasedNamespace;
    private readonly ValueLazy<NamespaceDecl> _namespace;

    internal NamespaceAliasDecl(CXCursor handle) : base(handle, CXCursor_NamespaceAlias, CX_DeclKind_NamespaceAlias)
    {
        _aliasedNamespace = new ValueLazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetSubDecl(0)));
        _namespace = new ValueLazy<NamespaceDecl>(() => TranslationUnit.GetOrCreate<NamespaceDecl>(Handle.GetSubDecl(1)));
    }

    public NamedDecl AliasedNamespace => _aliasedNamespace.Value;

    public new NamespaceAliasDecl CanonicalDecl => (NamespaceAliasDecl)base.CanonicalDecl;

    public NamespaceDecl Namespace => _namespace.Value;
}
