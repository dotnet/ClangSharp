// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ClassTemplateDecl : RedeclarableTemplateDecl
    {
        private readonly Lazy<Type> _injectedClassNameSpecialization;
        private readonly Lazy<IReadOnlyList<ClassTemplateSpecializationDecl>> _specializations;

        internal ClassTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ClassTemplate, CX_DeclKind.CX_DeclKind_ClassTemplate)
        {
            _injectedClassNameSpecialization = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.InjectedSpecializationType));

            _specializations = new Lazy<IReadOnlyList<ClassTemplateSpecializationDecl>>(() => {
                var numSpecializations = Handle.NumSpecializations;
                var specializations = new List<ClassTemplateSpecializationDecl>(numSpecializations);

                for (var i = 0; i <numSpecializations; i++)
                {
                    var specialization = TranslationUnit.GetOrCreate<ClassTemplateSpecializationDecl>(Handle.GetSpecialization(unchecked((uint)i)));
                    specializations.Add(specialization);
                }

                return specializations;
            });
        }

        public new ClassTemplateDecl CanonicalDecl => (ClassTemplateDecl)base.CanonicalDecl;

        public Type InjectedClassNameSpecialization => _injectedClassNameSpecialization.Value;

        public new ClassTemplateDecl InstantiatedFromMemberTemplate => (ClassTemplateDecl)base.InstantiatedFromMemberTemplate;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public new ClassTemplateDecl MostRecentDecl => (ClassTemplateDecl)base.MostRecentDecl;

        public new ClassTemplateDecl PreviousDecl => (ClassTemplateDecl)base.PreviousDecl;

        public IReadOnlyList<ClassTemplateSpecializationDecl> Specializations => _specializations.Value;

        public new CXXRecordDecl TemplatedDecl => (CXXRecordDecl)base.TemplatedDecl;
    }
}
