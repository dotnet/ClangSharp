// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class FriendDecl : Decl
{
    private readonly ValueLazy<NamedDecl> _friendNamedDecl;
    private readonly LazyList<LazyList<NamedDecl>> _friendTypeTemplateParameterLists;

    internal FriendDecl(CXCursor handle) : base(handle, CXCursor_FriendDecl, CX_DeclKind_Friend)
    {
        _friendNamedDecl = new ValueLazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.FriendDecl));
        _friendTypeTemplateParameterLists = LazyList.Create<LazyList<NamedDecl>>(Handle.NumTemplateParameterLists, (listIndex) => {
            var numTemplateParameters = Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
            return LazyList.Create<NamedDecl>(numTemplateParameters, (parameterIndex) => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)listIndex), unchecked((uint)parameterIndex))));
        });
    }

    public NamedDecl FriendNamedDecl => _friendNamedDecl.Value;

    public bool IsUnsupportedFriend => Handle.IsUnsupportedFriend;

    public uint FriendTypeNumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

    public IReadOnlyList<IReadOnlyList<NamedDecl>> FriendTypeTemplateParameterLists => _friendTypeTemplateParameterLists;
}
