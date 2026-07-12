// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ObjCInterfaceType : ObjCObjectType
{
    private ValueLazy<ObjCInterfaceType, ObjCInterfaceDecl> _decl;

    internal unsafe ObjCInterfaceType(CXType handle) : base(handle, CXType_ObjCInterface, CX_TypeClass_ObjCInterface)
    {
        _decl = new ValueLazy<ObjCInterfaceType, ObjCInterfaceDecl>(&DeclFactory);
    }

    public ObjCInterfaceDecl Decl => _decl.GetValue(this);

    private static unsafe ObjCInterfaceDecl DeclFactory(ObjCInterfaceType self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.Declaration);
}
