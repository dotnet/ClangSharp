// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ObjCTypeParamType : Type
{
    private readonly Lazy<ObjCTypeParamDecl> _decl;

    internal ObjCTypeParamType(CXType handle) : base(handle, CXType_ObjCTypeParam, CX_TypeClass_ObjCTypeParam)
    {
        _decl = new Lazy<ObjCTypeParamDecl>(() => TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.Declaration));
    }

    public ObjCTypeParamDecl Decl => _decl.Value;
}
