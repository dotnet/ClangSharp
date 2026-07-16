// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class UsingType : TypeWithKeyword
{
    private ValueLazy<UsingType, UsingShadowDecl> _foundDecl;

    internal unsafe UsingType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_Using)
    {
        _foundDecl = new ValueLazy<UsingType, UsingShadowDecl>(&FoundDeclFactory);
    }

    public UsingShadowDecl FoundDecl => _foundDecl.GetValue(this);

    private static unsafe UsingShadowDecl FoundDeclFactory(UsingType self) => self.TranslationUnit.GetOrCreate<UsingShadowDecl>(self.Handle.Declaration);
}
