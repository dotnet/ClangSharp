// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class FriendDecl : Decl
{
    private ValueLazy<FriendDecl, NamedDecl> _friendNamedDecl;
    private readonly LazyList<LazyList<NamedDecl>> _friendTypeTemplateParameterLists;

    internal unsafe FriendDecl(CXCursor handle) : base(handle, CXCursor_FriendDecl, CX_DeclKind_Friend)
    {
        _friendNamedDecl = new ValueLazy<FriendDecl, NamedDecl>(&FriendNamedDeclFactory);
        _friendTypeTemplateParameterLists = CreateTemplateParameterLists(this);
    }

    public NamedDecl FriendNamedDecl => _friendNamedDecl.GetValue(this);

    public bool IsUnsupportedFriend => Handle.IsUnsupportedFriend;

    public uint FriendTypeNumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

    public IReadOnlyList<IReadOnlyList<NamedDecl>> FriendTypeTemplateParameterLists => _friendTypeTemplateParameterLists;

    private static unsafe NamedDecl FriendNamedDeclFactory(FriendDecl self) => self.TranslationUnit.GetOrCreate<NamedDecl>(self.Handle.FriendDecl);
}
