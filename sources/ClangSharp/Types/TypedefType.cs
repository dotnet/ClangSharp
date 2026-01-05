// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class TypedefType : Type
{
    private readonly ValueLazy<TypedefNameDecl> _decl;

    internal TypedefType(CXType handle) : base(handle, CXType_Typedef, CX_TypeClass_Typedef, CXType_ObjCClass, CXType_ObjCId, CXType_ObjCSel)
    {
        _decl = new ValueLazy<TypedefNameDecl>(() => TranslationUnit.GetOrCreate<TypedefNameDecl>(Handle.Declaration));
    }

    public TypedefNameDecl Decl => _decl.Value;
}
