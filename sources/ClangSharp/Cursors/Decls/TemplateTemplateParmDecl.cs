// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class TemplateTemplateParmDecl : TemplateDecl, ITemplateParmPosition
{
    private readonly Lazy<TemplateArgumentLoc> _defaultArgument;

    internal TemplateTemplateParmDecl(CXCursor handle) : base(handle, CXCursor_TemplateTemplateParameter, CX_DeclKind_TemplateTemplateParm)
    {
        _defaultArgument = new Lazy<TemplateArgumentLoc>(() => TranslationUnit.GetOrCreate(handle.GetTemplateArgumentLoc(0)));
    }

    public TemplateArgumentLoc DefaultArgument => _defaultArgument.Value;

    public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

    public uint Depth => unchecked((uint)Handle.TemplateTypeParmDepth);

    public uint Index => unchecked((uint)Handle.TemplateTypeParmIndex);

    public bool IsExpandedParameterPack => Handle.IsExpandedParameterPack;

    public bool IsPackExpansion => Handle.IsPackExpansion;

    public bool IsParameterPack => Handle.IsParameterPack;

    public uint Position => unchecked((uint)Handle.TemplateTypeParmPosition);
}
