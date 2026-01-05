// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ClassTemplateDecl : RedeclarableTemplateDecl
{
    private readonly ValueLazy<Type> _injectedClassNameSpecialization;
    private readonly LazyList<ClassTemplateSpecializationDecl> _specializations;

    internal ClassTemplateDecl(CXCursor handle) : base(handle, CXCursor_ClassTemplate, CX_DeclKind_ClassTemplate)
    {
        _injectedClassNameSpecialization = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.InjectedSpecializationType));
        _specializations = LazyList.Create<ClassTemplateSpecializationDecl>(Handle.NumSpecializations, (i) => TranslationUnit.GetOrCreate<ClassTemplateSpecializationDecl>(Handle.GetSpecialization(unchecked((uint)i))));
    }

    public new ClassTemplateDecl CanonicalDecl => (ClassTemplateDecl)base.CanonicalDecl;

    public Type InjectedClassNameSpecialization => _injectedClassNameSpecialization.Value;

    public new ClassTemplateDecl InstantiatedFromMemberTemplate => (ClassTemplateDecl)base.InstantiatedFromMemberTemplate;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public new ClassTemplateDecl MostRecentDecl => (ClassTemplateDecl)base.MostRecentDecl;

    public new ClassTemplateDecl PreviousDecl => (ClassTemplateDecl)base.PreviousDecl;

    public IReadOnlyList<ClassTemplateSpecializationDecl> Specializations => _specializations;

    public new CXXRecordDecl TemplatedDecl => (CXXRecordDecl)base.TemplatedDecl;
}
