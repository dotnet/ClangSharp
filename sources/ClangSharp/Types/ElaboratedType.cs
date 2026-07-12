// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ElaboratedType : TypeWithKeyword
{
    private ValueLazy<ElaboratedType, Type> _namedType;
    private ValueLazy<ElaboratedType, TagDecl?> _ownedTagDecl;

    internal unsafe ElaboratedType(CXType handle) : base(handle, CXType_Elaborated, CX_TypeClass_Elaborated, CXType_ObjCClass, CXType_ObjCId, CXType_ObjCSel)
    {
        _namedType = new ValueLazy<ElaboratedType, Type>(&NamedTypeFactory);
        _ownedTagDecl = new ValueLazy<ElaboratedType, TagDecl?>(&OwnedTagDeclFactory);
    }

    public Type NamedType => _namedType.GetValue(this);

    public TagDecl? OwnedTagDecl => _ownedTagDecl.GetValue(this);

    private static unsafe TagDecl? OwnedTagDeclFactory(ElaboratedType self) => !self.Handle.OwnedTagDecl.IsNull ?self.TranslationUnit.GetOrCreate<TagDecl>(self.Handle.OwnedTagDecl) : null;

    private static unsafe Type NamedTypeFactory(ElaboratedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.NamedType);
}
