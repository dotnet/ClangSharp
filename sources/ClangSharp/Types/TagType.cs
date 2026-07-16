// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public class TagType : TypeWithKeyword
{
    private ValueLazy<TagType, TagDecl> _decl;

    private protected unsafe TagType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _decl = new ValueLazy<TagType, TagDecl>(&DeclFactory);
    }

    public TagDecl Decl => _decl.GetValue(this);

    private static unsafe TagDecl DeclFactory(TagType self) => self.TranslationUnit.GetOrCreate<TagDecl>(self.Handle.Declaration);
}
