// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ClassTemplateDecl : RedeclarableTemplateDecl
{
    private ValueLazy<ClassTemplateDecl, Type> _injectedClassNameSpecialization;
    private readonly LazyList<ClassTemplateSpecializationDecl> _specializations;

    internal unsafe ClassTemplateDecl(CXCursor handle) : base(handle, CXCursor_ClassTemplate, CX_DeclKind_ClassTemplate)
    {
        _injectedClassNameSpecialization = new ValueLazy<ClassTemplateDecl, Type>(&InjectedClassNameSpecializationFactory);
        _specializations = LazyList.Create<ClassTemplateSpecializationDecl>(Handle.NumSpecializations, (i) => TranslationUnit.GetOrCreate<ClassTemplateSpecializationDecl>(Handle.GetSpecialization(unchecked((uint)i))));
    }

    public new ClassTemplateDecl CanonicalDecl => (ClassTemplateDecl)base.CanonicalDecl;

    public Type InjectedClassNameSpecialization => _injectedClassNameSpecialization.GetValue(this);

    public new ClassTemplateDecl InstantiatedFromMemberTemplate => (ClassTemplateDecl)base.InstantiatedFromMemberTemplate;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public new ClassTemplateDecl MostRecentDecl => (ClassTemplateDecl)base.MostRecentDecl;

    public new ClassTemplateDecl PreviousDecl => (ClassTemplateDecl)base.PreviousDecl;

    public IReadOnlyList<ClassTemplateSpecializationDecl> Specializations => _specializations;

    public new CXXRecordDecl TemplatedDecl => (CXXRecordDecl)base.TemplatedDecl;

    private static unsafe Type InjectedClassNameSpecializationFactory(ClassTemplateDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.InjectedSpecializationType);
}
