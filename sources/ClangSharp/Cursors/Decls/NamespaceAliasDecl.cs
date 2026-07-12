// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class NamespaceAliasDecl : NamedDecl, IRedeclarable<NamespaceDecl>
{
    private ValueLazy<NamespaceAliasDecl, NamedDecl> _aliasedNamespace;
    private ValueLazy<NamespaceAliasDecl, NamespaceDecl> _namespace;

    internal unsafe NamespaceAliasDecl(CXCursor handle) : base(handle, CXCursor_NamespaceAlias, CX_DeclKind_NamespaceAlias)
    {
        _aliasedNamespace = new ValueLazy<NamespaceAliasDecl, NamedDecl>(&AliasedNamespaceFactory);
        _namespace = new ValueLazy<NamespaceAliasDecl, NamespaceDecl>(&NamespaceFactory);
    }

    public NamedDecl AliasedNamespace => _aliasedNamespace.GetValue(this);

    public new NamespaceAliasDecl CanonicalDecl => (NamespaceAliasDecl)base.CanonicalDecl;

    public NamespaceDecl Namespace => _namespace.GetValue(this);

    private static unsafe NamespaceDecl NamespaceFactory(NamespaceAliasDecl self) => self.TranslationUnit.GetOrCreate<NamespaceDecl>(self.Handle.GetSubDecl(1));

    private static unsafe NamedDecl AliasedNamespaceFactory(NamespaceAliasDecl self) => self.TranslationUnit.GetOrCreate<NamedDecl>(self.Handle.GetSubDecl(0));
}
