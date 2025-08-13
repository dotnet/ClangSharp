// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class FriendTemplateDecl : Decl
{
    private readonly Lazy<NamedDecl> _friendDecl;
    private readonly Lazy<Type> _friendType;
    private readonly LazyList<LazyList<NamedDecl>> _templateParameterLists;

    internal FriendTemplateDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_FriendTemplate)
    {
        _friendDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.FriendDecl));
        _friendType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        _templateParameterLists = LazyList.Create<LazyList<NamedDecl>>(Handle.NumTemplateParameterLists, (listIndex) => {
            var numTemplateParameters = Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
            return LazyList.Create<NamedDecl>(numTemplateParameters, (parameterIndex) => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)listIndex), unchecked((uint)parameterIndex))));
        });
    }

    public NamedDecl FriendDecl => _friendDecl.Value;

    public Type FriendType => _friendType.Value;

    public uint NumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

    public IReadOnlyList<IReadOnlyList<NamedDecl>> TemplateParameterLists => _templateParameterLists;
}
