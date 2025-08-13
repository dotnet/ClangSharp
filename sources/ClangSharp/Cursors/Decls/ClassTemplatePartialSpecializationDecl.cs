// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;
using System;
using System.Collections.Generic;

namespace ClangSharp;

public sealed class ClassTemplatePartialSpecializationDecl : ClassTemplateSpecializationDecl
{
    private readonly LazyList<Expr> _associatedConstraints;
    private readonly Lazy<Type> _injectedSpecializationType;
    private readonly Lazy<ClassTemplatePartialSpecializationDecl> _instantiatedFromMember;
    private readonly LazyList<NamedDecl> _templateParameters;

    internal ClassTemplatePartialSpecializationDecl(CXCursor handle) : base(handle, CXCursor_ClassTemplatePartialSpecialization, CX_DeclKind_ClassTemplatePartialSpecialization)
    {
        _associatedConstraints = LazyList.Create<Expr>(Handle.NumAssociatedConstraints, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetAssociatedConstraint(unchecked((uint)i))));
        _injectedSpecializationType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.InjectedSpecializationType));
        _instantiatedFromMember = new Lazy<ClassTemplatePartialSpecializationDecl>(() => TranslationUnit.GetOrCreate<ClassTemplatePartialSpecializationDecl>(Handle.InstantiatedFromMember));
        _templateParameters = LazyList.Create<NamedDecl>(Handle.GetNumTemplateParameters(0), (i) => TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(0, unchecked((uint)i))));
    }

    public IReadOnlyList<Expr> AssociatedConstraints => _associatedConstraints;

    public bool HasAssociatedConstraints => Handle.NumAssociatedConstraints != 0;

    public Type InjectedSpecializationType => _injectedSpecializationType.Value;

    public ClassTemplatePartialSpecializationDecl InstantiatedFromMember => _instantiatedFromMember.Value;

    public ClassTemplatePartialSpecializationDecl InstantiatedFromMemberTemplate => InstantiatedFromMember;

    public bool IsMemberSpecialization => Handle.IsMemberSpecialization;

    public new ClassTemplatePartialSpecializationDecl MostRecentDecl => (ClassTemplatePartialSpecializationDecl)base.MostRecentDecl;

    public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters;
}
