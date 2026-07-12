// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class NamespaceDecl : NamedDecl, IDeclContext, IRedeclarable<NamespaceDecl>
{
    private ValueLazy<NamespaceDecl, NamespaceDecl> _anonymousNamespace;
    private ValueLazy<NamespaceDecl, NamespaceDecl> _originalNamespace;

    internal unsafe NamespaceDecl(CXCursor handle) : base(handle, CXCursor_Namespace, CX_DeclKind_Namespace)
    {
        _anonymousNamespace = new ValueLazy<NamespaceDecl, NamespaceDecl>(&AnonymousNamespaceFactory);
        _originalNamespace = new ValueLazy<NamespaceDecl, NamespaceDecl>(&OriginalNamespaceFactory);
    }

    public NamespaceDecl AnonymousNamespace => _anonymousNamespace.GetValue(this);

    public new NamespaceDecl CanonicalDecl => (NamespaceDecl)base.CanonicalDecl;

    public bool IsAnonymousNamespace => Handle.IsAnonymous;

    public bool IsInline => Handle.IsInlineNamespace;

    public NamespaceDecl OriginalNamespace => _originalNamespace.GetValue(this);

    private static unsafe NamespaceDecl OriginalNamespaceFactory(NamespaceDecl self) => self.TranslationUnit.GetOrCreate<NamespaceDecl>(self.Handle.GetSubDecl(1));

    private static unsafe NamespaceDecl AnonymousNamespaceFactory(NamespaceDecl self) => self.TranslationUnit.GetOrCreate<NamespaceDecl>(self.Handle.GetSubDecl(0));
}
