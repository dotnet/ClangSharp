// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ObjCTypeParamType : Type
{
    private ValueLazy<ObjCTypeParamType, ObjCTypeParamDecl> _decl;
    private readonly LazyList<ObjCProtocolDecl> _protocols;

    internal unsafe ObjCTypeParamType(CXType handle) : base(handle, CXType_ObjCTypeParam, CX_TypeClass_ObjCTypeParam)
    {
        _decl = new ValueLazy<ObjCTypeParamType, ObjCTypeParamDecl>(&DeclFactory);
        _protocols = LazyList.Create<ObjCProtocolDecl>(this, Handle.NumObjCTypeParamProtocols, &ProtocolsFactory);
    }

    public ObjCTypeParamDecl Decl => _decl.GetValue(this);

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    private static unsafe ObjCTypeParamDecl DeclFactory(ObjCTypeParamType self) => self.TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(self.Handle.Declaration);

    private static unsafe ObjCProtocolDecl ProtocolsFactory(object self, int i)
    {
        var @this = (ObjCTypeParamType)self;
        return @this.TranslationUnit.GetOrCreate<ObjCProtocolDecl>(@this.Handle.GetObjCTypeParamProtocol(unchecked((uint)i)));
    }
}
