// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class FriendTemplateDecl : Decl
{
    private ValueLazy<FriendTemplateDecl, NamedDecl> _friendDecl;
    private ValueLazy<FriendTemplateDecl, Type> _friendType;
    private readonly LazyList<LazyList<NamedDecl>> _templateParameterLists;

    internal unsafe FriendTemplateDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_FriendTemplate)
    {
        _friendDecl = new ValueLazy<FriendTemplateDecl, NamedDecl>(&FriendDeclFactory);
        _friendType = new ValueLazy<FriendTemplateDecl, Type>(&FriendTypeFactory);
        _templateParameterLists = LazyList.Create<LazyList<NamedDecl>>(Handle.NumTemplateParameterLists, (listIndex) => {
            var numTemplateParameters = Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
            return LazyList.Create<NamedDecl>(numTemplateParameters, (parameterIndex) => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)listIndex), unchecked((uint)parameterIndex))));
        });
    }

    public NamedDecl FriendDecl => _friendDecl.GetValue(this);

    public Type FriendType => _friendType.GetValue(this);

    public uint NumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

    public IReadOnlyList<IReadOnlyList<NamedDecl>> TemplateParameterLists => _templateParameterLists;

    private static unsafe Type FriendTypeFactory(FriendTemplateDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);

    private static unsafe NamedDecl FriendDeclFactory(FriendTemplateDecl self) => self.TranslationUnit.GetOrCreate<NamedDecl>(self.Handle.FriendDecl);
}
