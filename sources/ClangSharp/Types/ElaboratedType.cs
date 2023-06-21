// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ElaboratedType : TypeWithKeyword
{
    private readonly Lazy<Type> _namedType;
    private readonly Lazy<TagDecl?> _ownedTagDecl;

    internal ElaboratedType(CXType handle) : base(handle, CXType_Elaborated, CX_TypeClass_Elaborated)
    {
        _namedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.NamedType));
        _ownedTagDecl = new Lazy<TagDecl?>(() => !Handle.OwnedTagDecl.IsNull ?TranslationUnit.GetOrCreate<TagDecl>(Handle.OwnedTagDecl) : null);
    }

    public Type NamedType => _namedType.Value;

    public TagDecl? OwnedTagDecl => _ownedTagDecl.Value;
}
