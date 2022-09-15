// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class NamespaceAliasDecl : NamedDecl, IRedeclarable<NamespaceDecl>
{
    private readonly Lazy<NamedDecl> _aliasedNamespace;
    private readonly Lazy<NamespaceDecl> _namespace;

    internal NamespaceAliasDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_NamespaceAlias, CX_DeclKind.CX_DeclKind_NamespaceAlias)
    {
        _aliasedNamespace = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetSubDecl(0)));
        _namespace = new Lazy<NamespaceDecl>(() => TranslationUnit.GetOrCreate<NamespaceDecl>(Handle.GetSubDecl(1)));
    }

    public NamedDecl AliasedNamespace => _aliasedNamespace.Value;

    public new NamespaceAliasDecl CanonicalDecl => (NamespaceAliasDecl)base.CanonicalDecl;

    public NamespaceDecl Namespace => _namespace.Value;
}
