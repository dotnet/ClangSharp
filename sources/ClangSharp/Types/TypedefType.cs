// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class TypedefType : TypeWithKeyword
{
    private ValueLazy<TypedefType, TypedefNameDecl> _decl;

    internal unsafe TypedefType(CXType handle) : base(handle, CXType_Typedef, CX_TypeClass_Typedef, CXType_ObjCClass, CXType_ObjCId, CXType_ObjCSel)
    {
        _decl = new ValueLazy<TypedefType, TypedefNameDecl>(&DeclFactory);
    }

    public TypedefNameDecl Decl => _decl.GetValue(this);

    private static unsafe TypedefNameDecl DeclFactory(TypedefType self) => self.TranslationUnit.GetOrCreate<TypedefNameDecl>(self.Handle.Declaration);
}
