// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    public class ClassTemplateSpecializationDecl : CXXRecordDecl
    {
        private readonly Lazy<ClassTemplateDecl> _specializedTemplate;
        private readonly Lazy<IReadOnlyList<TemplateArgument>> _templateArgs;

        internal ClassTemplateSpecializationDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind.CX_DeclKind_ClassTemplateSpecialization)
        {
            _specializedTemplate = new Lazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.SpecializedCursorTemplate));
            _templateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (int i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        private protected ClassTemplateSpecializationDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastClassTemplateSpecialization < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstClassTemplateSpecialization))
            {
                throw new ArgumentException(nameof(handle));
            }

            _specializedTemplate = new Lazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.SpecializedCursorTemplate));
            _templateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (int i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public bool IsClassScopeExplicitSpecialization => IsExplicitSpecialization && (LexicalDeclContext is CXXRecordDecl);

        public bool IsExplicitInstantiationOrSpecialization
        {
            get
            {
                switch (SpecializationKind)
                {
                    case CX_TemplateSpecializationKind.CX_TSK_ExplicitSpecialization:
                    case CX_TemplateSpecializationKind.CX_TSK_ExplicitInstantiationDeclaration:
                    case CX_TemplateSpecializationKind.CX_TSK_ExplicitInstantiationDefinition:
                    {
                        return true;
                    }

                    case CX_TemplateSpecializationKind.CX_TSK_Undeclared:
                    case CX_TemplateSpecializationKind.CX_TSK_ImplicitInstantiation:
                    {
                        return false;
                    }
                }

                Debug.Fail("bad template specialization kind");
                return false;
            }
        }

        public bool IsExplicitSpecialization => SpecializationKind == CX_TemplateSpecializationKind.CX_TSK_ExplicitSpecialization;

        public new ClassTemplateSpecializationDecl MostRecentDecl => (ClassTemplateSpecializationDecl)base.MostRecentDecl;

        public CX_TemplateSpecializationKind SpecializationKind => Handle.TemplateSpecializationKind;

        public ClassTemplateDecl SpecializedTemplate => _specializedTemplate.Value;

        public IReadOnlyList<TemplateArgument> TemplateArgs => _templateArgs.Value;
    }
}
