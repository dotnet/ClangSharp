// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class NamespaceDecl : NamedDecl, IDeclContext, IRedeclarable<NamespaceDecl>
{
    private readonly Lazy<NamespaceDecl> _anonymousNamespace;
    private readonly Lazy<NamespaceDecl> _originalNamespace;

    internal NamespaceDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Namespace, CX_DeclKind.CX_DeclKind_Namespace)
    {
        _anonymousNamespace = new Lazy<NamespaceDecl>(() => TranslationUnit.GetOrCreate<NamespaceDecl>(Handle.GetSubDecl(0)));
        _originalNamespace = new Lazy<NamespaceDecl>(() => TranslationUnit.GetOrCreate<NamespaceDecl>(Handle.GetSubDecl(1)));
    }

    public NamespaceDecl AnonymousNamespace => _anonymousNamespace.Value;

    public new NamespaceDecl CanonicalDecl => (NamespaceDecl)base.CanonicalDecl;

    public bool IsAnonymousNamespace => Handle.IsAnonymous;

    public bool IsInline => Handle.IsInlineNamespace;

    public NamespaceDecl OriginalNamespace => _originalNamespace.Value;
}
