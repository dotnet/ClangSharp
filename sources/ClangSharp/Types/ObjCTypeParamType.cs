// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ObjCTypeParamType : Type
{
    private ValueLazy<ObjCTypeParamType, ObjCTypeParamDecl> _decl;

    internal unsafe ObjCTypeParamType(CXType handle) : base(handle, CXType_ObjCTypeParam, CX_TypeClass_ObjCTypeParam)
    {
        _decl = new ValueLazy<ObjCTypeParamType, ObjCTypeParamDecl>(&DeclFactory);
    }

    public ObjCTypeParamDecl Decl => _decl.GetValue(this);

    private static unsafe ObjCTypeParamDecl DeclFactory(ObjCTypeParamType self) => self.TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(self.Handle.Declaration);
}
