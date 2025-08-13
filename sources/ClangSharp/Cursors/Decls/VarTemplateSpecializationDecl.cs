// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class VarTemplateSpecializationDecl : VarDecl
{
    private readonly ValueLazy<VarTemplateDecl> _specializedTemplate;
    private readonly LazyList<TemplateArgument> _templateArgs;

    internal VarTemplateSpecializationDecl(CXCursor handle) : this(handle, CXCursor_UnexposedDecl, CX_DeclKind_VarTemplateSpecialization)
    {
    }

    private protected VarTemplateSpecializationDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastVarTemplateSpecialization or < CX_DeclKind_FirstVarTemplateSpecialization)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _specializedTemplate = new ValueLazy<VarTemplateDecl>(() => TranslationUnit.GetOrCreate<VarTemplateDecl>(Handle.SpecializedCursorTemplate));
        _templateArgs = LazyList.Create<TemplateArgument>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i))));
    }

    public new VarTemplateSpecializationDecl MostRecentDecl => (VarTemplateSpecializationDecl)base.MostRecentDecl;

    public VarTemplateDecl SpecializedTemplate => _specializedTemplate.Value;

    public IReadOnlyList<TemplateArgument> TemplateArgs => _templateArgs;
}
