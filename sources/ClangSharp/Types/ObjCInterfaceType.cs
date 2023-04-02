// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ObjCInterfaceType : ObjCObjectType
{
    private readonly Lazy<ObjCInterfaceDecl> _decl;

    internal ObjCInterfaceType(CXType handle) : base(handle, CXType_ObjCObject, CX_TypeClass_ObjCInterface)
    {
        _decl = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.Declaration));
    }

    public ObjCInterfaceDecl Decl => _decl.Value;
}
