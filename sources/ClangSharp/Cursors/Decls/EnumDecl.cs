// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class EnumDecl : TagDecl
{
    private readonly LazyList<EnumConstantDecl> _enumerators;
    private readonly ValueLazy<EnumDecl> _instantiatedFromMemberEnum;
    private readonly ValueLazy<Type> _integerType;
    private readonly ValueLazy<Type> _promotionType;
    private readonly ValueLazy<EnumDecl> _templateInstantiationPattern;

    internal EnumDecl(CXCursor handle) : base(handle, CXCursor_EnumDecl, CX_DeclKind_Enum)
    {
        _enumerators = LazyList.Create<EnumConstantDecl>(Handle.NumEnumerators, (i) => TranslationUnit.GetOrCreate<EnumConstantDecl>(Handle.GetEnumerator(unchecked((uint)i))));
        _instantiatedFromMemberEnum = new ValueLazy<EnumDecl>(() => TranslationUnit.GetOrCreate<EnumDecl>(Handle.InstantiatedFromMember));
        _integerType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.EnumDecl_IntegerType));
        _promotionType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.EnumDecl_PromotionType));
        _templateInstantiationPattern = new ValueLazy<EnumDecl>(() => TranslationUnit.GetOrCreate<EnumDecl>(Handle.TemplateInstantiationPattern));
    }

    public new EnumDecl CanonicalDecl => (EnumDecl)base.CanonicalDecl;

    public new EnumDecl? Definition => (EnumDecl?)base.Definition;

    public IReadOnlyList<EnumConstantDecl> Enumerators => _enumerators;

    public EnumDecl InstantiatedFromMemberEnum => _instantiatedFromMemberEnum.Value;

    public Type IntegerType => _integerType.Value;

    public bool IsComplete => IsCompleteDefinition || (IntegerType is not null);

    public bool IsScoped => Handle.EnumDecl_IsScoped;

    public new EnumDecl MostRecentDecl => (EnumDecl)base.MostRecentDecl;

    public new EnumDecl PreviousDecl => (EnumDecl)base.PreviousDecl;

    public Type PromotionType => _promotionType.Value;

    public EnumDecl TemplateInstantiationPattern => _templateInstantiationPattern.Value;

    public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;
}
