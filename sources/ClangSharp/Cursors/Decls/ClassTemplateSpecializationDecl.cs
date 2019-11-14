// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;
using System;
using System.Collections.Generic;

namespace ClangSharp
{
    public class ClassTemplateSpecializationDecl : CXXRecordDecl
    {
        private readonly Lazy<ClassTemplateDecl> _specializedTemplate;
        private readonly Lazy<IReadOnlyList<Type>> _templateArgs;

        internal ClassTemplateSpecializationDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind.CX_DeclKind_ClassTemplateSpecialization)
        {
        }

        private protected ClassTemplateSpecializationDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastClassTemplateSpecialization < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstClassTemplateSpecialization))
            {
                throw new ArgumentException(nameof(handle));
            }

            _specializedTemplate = new Lazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.SpecializedCursorTemplate));
            _templateArgs = new Lazy<IReadOnlyList<Type>>(() => {
                var templateArgsSize = TemplateArgsSize;
                var templateArgs = new List<Type>(templateArgsSize);

                for (var index = 0; index < templateArgsSize; index++)
                {
                    var templateArg = TypeForDecl.Handle.GetTemplateArgumentAsType((uint)index);
                    templateArgs.Add(TranslationUnit.GetOrCreate<Type>(templateArg));
                }

                return templateArgs;
            });
        }

        public IReadOnlyList<Type> TemplateArgs => _templateArgs.Value;

        public int TemplateArgsSize => TypeForDecl.Handle.NumTemplateArguments;

        public ClassTemplateDecl SpecializedTemplate => _specializedTemplate.Value;
    }
}
