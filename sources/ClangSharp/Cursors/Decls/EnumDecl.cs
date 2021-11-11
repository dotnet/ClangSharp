// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class EnumDecl : TagDecl
    {
        private readonly Lazy<IReadOnlyList<EnumConstantDecl>> _enumerators;
        private readonly Lazy<EnumDecl> _instantiatedFromMemberEnum;
        private readonly Lazy<Type> _integerType;
        private readonly Lazy<Type> _promotionType;
        private readonly Lazy<EnumDecl> _templateInstantiationPattern;

        internal EnumDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_EnumDecl, CX_DeclKind.CX_DeclKind_Enum)
        {
            _enumerators = new Lazy<IReadOnlyList<EnumConstantDecl>>(() => {
                var numEnumerators = Handle.NumEnumerators;
                var enumerators = new List<EnumConstantDecl>(numEnumerators);

                for (var i = 0; i < numEnumerators; i++)
                {
                    var enumerator = TranslationUnit.GetOrCreate<EnumConstantDecl>(Handle.GetEnumerator(unchecked((uint)i)));
                    enumerators.Add(enumerator);
                }

                return enumerators;
            });

            _instantiatedFromMemberEnum = new Lazy<EnumDecl>(() => TranslationUnit.GetOrCreate<EnumDecl>(Handle.InstantiatedFromMember));
            _integerType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.EnumDecl_IntegerType));
            _promotionType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.EnumDecl_PromotionType));
            _templateInstantiationPattern = new Lazy<EnumDecl>(() => TranslationUnit.GetOrCreate<EnumDecl>(Handle.TemplateInstantiationPattern));
        }

        public new EnumDecl CanonicalDecl => (EnumDecl)base.CanonicalDecl;

        public new EnumDecl Definition => (EnumDecl)base.Definition;

        public IReadOnlyList<EnumConstantDecl> Enumerators => _enumerators.Value;

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
}
