// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class TemplateTypeParmType : Type
{
    private ValueLazy<TemplateTypeParmType, TemplateTypeParmDecl> _decl;

    internal unsafe TemplateTypeParmType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_TemplateTypeParm)
    {
        _decl = new ValueLazy<TemplateTypeParmType, TemplateTypeParmDecl>(&DeclFactory);
    }

    public TemplateTypeParmDecl Decl => _decl.GetValue(this);

    public uint Depth => unchecked((uint)Handle.Depth);

    public uint Index => unchecked((uint)Handle.Index);

    public bool IsParameterPack => Handle.IsParameterPack;

    private static unsafe TemplateTypeParmDecl DeclFactory(TemplateTypeParmType self) => self.TranslationUnit.GetOrCreate<TemplateTypeParmDecl>(self.Handle.Declaration);
}
