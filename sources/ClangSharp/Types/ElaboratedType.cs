// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ElaboratedType : TypeWithKeyword
{
    private readonly ValueLazy<Type> _namedType;
    private readonly ValueLazy<TagDecl?> _ownedTagDecl;

    internal ElaboratedType(CXType handle) : base(handle, CXType_Elaborated, CX_TypeClass_Elaborated, CXType_ObjCClass, CXType_ObjCId, CXType_ObjCSel)
    {
        _namedType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.NamedType));
        _ownedTagDecl = new ValueLazy<TagDecl?>(() => !Handle.OwnedTagDecl.IsNull ?TranslationUnit.GetOrCreate<TagDecl>(Handle.OwnedTagDecl) : null);
    }

    public Type NamedType => _namedType.Value;

    public TagDecl? OwnedTagDecl => _ownedTagDecl.Value;
}
