// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class VarTemplatePartialSpecializationDecl : VarDecl
{
    private readonly LazyList<Expr> _associatedConstraints;
    private readonly ValueLazy<VarTemplatePartialSpecializationDecl> _instantiatedFromMember;
    private readonly LazyList<NamedDecl> _templateParameters;

    internal VarTemplatePartialSpecializationDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_VarTemplatePartialSpecialization)
    {
        _associatedConstraints = LazyList.Create<Expr>(Handle.NumAssociatedConstraints, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetAssociatedConstraint(unchecked((uint)i))));
        _instantiatedFromMember = new ValueLazy<VarTemplatePartialSpecializationDecl>(() => TranslationUnit.GetOrCreate<VarTemplatePartialSpecializationDecl>(Handle.InstantiatedFromMember));
        _templateParameters = LazyList.Create<NamedDecl>(Handle.GetNumTemplateParameters(0), (i) => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(0, unchecked((uint)i))));
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public VarTemplatePartialSpecializationDecl InstantiatedFromMember => _instantiatedFromMember.Value;

    public new VarTemplatePartialSpecializationDecl MostRecentDecl => (VarTemplatePartialSpecializationDecl)base.MostRecentDecl;

    public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters;
}
