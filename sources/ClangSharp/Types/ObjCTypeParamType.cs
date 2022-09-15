// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCTypeParamType : Type
{
    private readonly Lazy<ObjCTypeParamDecl> _decl;

    internal ObjCTypeParamType(CXType handle) : base(handle, CXTypeKind.CXType_ObjCTypeParam, CX_TypeClass.CX_TypeClass_ObjCTypeParam)
    {
        _decl = new Lazy<ObjCTypeParamDecl>(() => TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.Declaration));
    }

    public ObjCTypeParamDecl Decl => _decl.Value;
}
