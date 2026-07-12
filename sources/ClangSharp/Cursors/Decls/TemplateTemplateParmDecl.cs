// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class TemplateTemplateParmDecl : TemplateDecl, ITemplateParmPosition
{
    private ValueLazy<TemplateTemplateParmDecl, TemplateArgumentLoc> _defaultArgument;

    internal unsafe TemplateTemplateParmDecl(CXCursor handle) : base(handle, CXCursor_TemplateTemplateParameter, CX_DeclKind_TemplateTemplateParm)
    {
        _defaultArgument = new ValueLazy<TemplateTemplateParmDecl, TemplateArgumentLoc>(&DefaultArgumentFactory);
    }

    public TemplateArgumentLoc DefaultArgument => _defaultArgument.GetValue(this);

    public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

    public uint Depth => unchecked((uint)Handle.TemplateTypeParmDepth);

    public uint Index => unchecked((uint)Handle.TemplateTypeParmIndex);

    public bool IsExpandedParameterPack => Handle.IsExpandedParameterPack;

    public bool IsPackExpansion => Handle.IsPackExpansion;

    public bool IsParameterPack => Handle.IsParameterPack;

    public uint Position => unchecked((uint)Handle.TemplateTypeParmPosition);

    private static unsafe TemplateArgumentLoc DefaultArgumentFactory(TemplateTemplateParmDecl self) => self.TranslationUnit.GetOrCreate(self.Handle.GetTemplateArgumentLoc(0));
}
