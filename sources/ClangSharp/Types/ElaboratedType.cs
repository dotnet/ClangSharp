// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ElaboratedType : TypeWithKeyword
{
    private readonly Lazy<Type> _namedType;
    private readonly Lazy<TagDecl> _ownedTagDecl;

    internal ElaboratedType(CXType handle) : base(handle, CXTypeKind.CXType_Elaborated, CX_TypeClass.CX_TypeClass_Elaborated)
    {
        _namedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.NamedType));
        _ownedTagDecl = new Lazy<TagDecl>(() => TranslationUnit.GetOrCreate<TagDecl>(Handle.OwnedTagDecl));
    }

    public Type NamedType => _namedType.Value;

    public TagDecl OwnedTagDecl => _ownedTagDecl.Value;
}
