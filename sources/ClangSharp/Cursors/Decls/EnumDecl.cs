// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class EnumDecl : TagDecl
{
    private readonly LazyList<EnumConstantDecl> _enumerators;
    private ValueLazy<EnumDecl, EnumDecl> _instantiatedFromMemberEnum;
    private ValueLazy<EnumDecl, Type> _integerType;
    private ValueLazy<EnumDecl, Type> _promotionType;
    private ValueLazy<EnumDecl, EnumDecl> _templateInstantiationPattern;

    internal unsafe EnumDecl(CXCursor handle) : base(handle, CXCursor_EnumDecl, CX_DeclKind_Enum)
    {
        _enumerators = LazyList.Create<EnumConstantDecl>(Handle.NumEnumerators, (i) => TranslationUnit.GetOrCreate<EnumConstantDecl>(Handle.GetEnumerator(unchecked((uint)i))));
        _instantiatedFromMemberEnum = new ValueLazy<EnumDecl, EnumDecl>(&InstantiatedFromMemberEnumFactory);
        _integerType = new ValueLazy<EnumDecl, Type>(&IntegerTypeFactory);
        _promotionType = new ValueLazy<EnumDecl, Type>(&PromotionTypeFactory);
        _templateInstantiationPattern = new ValueLazy<EnumDecl, EnumDecl>(&TemplateInstantiationPatternFactory);
    }

    public new EnumDecl CanonicalDecl => (EnumDecl)base.CanonicalDecl;

    public new EnumDecl? Definition => (EnumDecl?)base.Definition;

    public IReadOnlyList<EnumConstantDecl> Enumerators => _enumerators;

    public EnumDecl InstantiatedFromMemberEnum => _instantiatedFromMemberEnum.GetValue(this);

    public Type IntegerType => _integerType.GetValue(this);

    public bool IsComplete => IsCompleteDefinition || (IntegerType is not null);

    public bool IsScoped => Handle.EnumDecl_IsScoped;

    public new EnumDecl MostRecentDecl => (EnumDecl)base.MostRecentDecl;

    public new EnumDecl PreviousDecl => (EnumDecl)base.PreviousDecl;

    public Type PromotionType => _promotionType.GetValue(this);

    public EnumDecl TemplateInstantiationPattern => _templateInstantiationPattern.GetValue(this);

    public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;

    private static unsafe EnumDecl TemplateInstantiationPatternFactory(EnumDecl self) => self.TranslationUnit.GetOrCreate<EnumDecl>(self.Handle.TemplateInstantiationPattern);

    private static unsafe Type PromotionTypeFactory(EnumDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.EnumDecl_PromotionType);

    private static unsafe Type IntegerTypeFactory(EnumDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.EnumDecl_IntegerType);

    private static unsafe EnumDecl InstantiatedFromMemberEnumFactory(EnumDecl self) => self.TranslationUnit.GetOrCreate<EnumDecl>(self.Handle.InstantiatedFromMember);
}
