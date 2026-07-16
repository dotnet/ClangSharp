// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class UnresolvedUsingType : TypeWithKeyword
{
    private ValueLazy<UnresolvedUsingType, UnresolvedUsingTypenameDecl> _decl;

    internal unsafe UnresolvedUsingType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_UnresolvedUsing)
    {
        _decl = new ValueLazy<UnresolvedUsingType, UnresolvedUsingTypenameDecl>(&DeclFactory);
    }

    public UnresolvedUsingTypenameDecl Decl => _decl.GetValue(this);

    private static unsafe UnresolvedUsingTypenameDecl DeclFactory(UnresolvedUsingType self) => self.TranslationUnit.GetOrCreate<UnresolvedUsingTypenameDecl>(self.Handle.Declaration);
}
